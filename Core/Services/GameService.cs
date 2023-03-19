using System.Data;
using Core.Dtos;
using Core.Entities;
using Core.Entities.Enums;
using Core.Model;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace Core.Services;

public class GameService
{
    public const char Nought = 'o';
    public const char Cross = 'x';
    public const char Empty = ' ';
    private readonly ApplicationContext _db;
    private readonly GameHelperService _helperService;
    private readonly IUsersService _usersService;

    public GameService(ApplicationContext context, IUsersService usersService, GameHelperService helperService)
    {
        _db = context;
        _usersService = usersService;
        _helperService = helperService;
    }

    public async Task<ICollection<GameItemDto>> GetGames(GetGamesRequestDto dto)
    {
        var currentUserId = _usersService.GetCurrentUserId();
        var games = await _db.Games.Where(g =>
            // ReSharper disable once SimplifyConditionalTernaryExpression
            (g.InvitingUser.Id == currentUserId || g.InvitedUser.Id == currentUserId) &&
            dto.ShowOnlyCurrentGames
                ? g.Status == GameStatus.Invited || g.Status == GameStatus.Accepted
                : true).Select(g => new GameItemDto
        {
            Id = g.Id,
            InvitingUserId = g.InvitingUser.Id,
            InvitedUserId = g.InvitedUser.Id,
            CountOfRounds = g.CountOfRounds,
            CurrentRoundNumber = g.Rounds.Count(),
            Status = g.Status,
            InvitingUserPlaysFirst = g.InvitingUserPlaysFirst,
            Result = g.Result
        }).ToListAsync();
        return games;
    }

    public async Task<OneOf<GameDto, BlErrorDto>> GetGame(GetGameRequestDto dto)
    {
        var currentUserId = _usersService.GetCurrentUserId();
        var game = await _db.Games.Where(g =>
                g.Id == dto.GameId && (g.InvitingUser.Id == currentUserId || g.InvitedUser.Id == currentUserId))
            .Select(g => new GameDto
            {
                Id = g.Id,
                InvitingUserId = g.InvitingUser.Id,
                InvitedUserId = g.InvitedUser.Id,
                CountOfRounds = g.CountOfRounds,
                Rounds = g.Rounds.Select(r => new GameRoundDto
                {
                    RoundNumber = r.RoundNumber,
                    Field = r.Field,
                    InvitingUserPlaysCrosses = r.InvitingUserPlaysCrosses,
                    Result = r.Result
                }).ToList(),
                Status = g.Status,
                InvitingUserPlaysFirst = g.InvitingUserPlaysFirst,
                Result = g.Result
            })
            .FirstOrDefaultAsync();
        if (game == null)
            return new BlErrorDto("GameNotFound", "Game not found");
        return game;
    }

    public async Task<OneOf<Success, BlErrorDto>> InviteUser(InviteModel model)
    {
        var currentUserId = _usersService.GetCurrentUserId();
        if (currentUserId == model.InvitedUserId)
            return new BlErrorDto("IncorrectId", "You can't invite to game yourself");
        var currentUser = await _db.Users.FirstAsync(u => u.Id == currentUserId);
        var invitedUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == model.InvitedUserId);
        if (invitedUser == null)
            return new BlErrorDto("NotFound", "User with this id not found");
        if (await _db.Games.Where(g => (g.Status == GameStatus.Invited || g.Status == GameStatus.Accepted) &&
                                       ((g.InvitingUser.Id == currentUserId &&
                                         g.InvitedUser.Id == model.InvitedUserId) ||
                                        (g.InvitingUser.Id == model.InvitedUserId &&
                                         g.InvitedUser.Id == currentUserId)))
                .AnyAsync())
            return new BlErrorDto("GameAlreadyExists", "Game with this user already exists");

        var invitingUserPlaysCrosses = Random.Shared.Next(2) == 0;

        _db.Games.Add(new Game
        {
            Status = GameStatus.Invited,
            InvitingUser = currentUser,
            InvitedUser = invitedUser,
            CountOfRounds = model.CountOfRounds,
            InvitingUserPlaysFirst = invitingUserPlaysCrosses,
            Rounds = new List<GameRound>
            {
                new()
                {
                    RoundNumber = 0,
                    Field = string.Join("", Enumerable.Repeat(' ', 9)),
                    InvitingUserPlaysCrosses = invitingUserPlaysCrosses,
                    Result = GameResult.NotOver
                }
            },
            Result = GameResult.NotOver
        });
        await _db.SaveChangesAsync();
        return new Success();
    }

    public async Task<OneOf<Success, BlErrorDto>> AnswerInvitation(InviteAnswerModel answer)
    {
        var currentUserId = _usersService.GetCurrentUserId();
        await _db.Database.BeginTransactionAsync(IsolationLevel.Snapshot);
        var game = await _db.Games
            .Where(g => g.InvitingUser.Id == answer.InvitingUserId && g.InvitedUser.Id == currentUserId &&
                        g.Status == GameStatus.Invited)
            .FirstOrDefaultAsync();
        if (game == null)
            return new BlErrorDto("GameNotFound", "Game with this user doesn't exists");
        game.Status = answer.Accept ? game.Status = GameStatus.Accepted : GameStatus.Declined;
        await _db.SaveChangesAsync();
        await _db.Database.CommitTransactionAsync();
        return new Success();
    }


    public async Task<OneOf<Success, BlErrorDto>> PlaceMark(PlaceMarkDto dto)
    {
        var currentUserId = _usersService.GetCurrentUserId();
        var game = await _db.Games.Where(g =>
                g.Id == dto.GameId && (g.InvitingUser.Id == currentUserId || g.InvitedUser.Id == currentUserId))
            .Include(g => g.InvitingUser)
            .Include(g => g.InvitedUser)
            .Include(g => g.Rounds)
            .FirstOrDefaultAsync();
        if (game == null)
            return new BlErrorDto("GameNotFound", "Game not found");
        if (game.Status != GameStatus.Accepted)
            return new BlErrorDto("GameHasEnded", "Game has ended or hasn't started");

        game.Rounds = game.Rounds.OrderBy(r => r.RoundNumber).ToList();
        var currentRound = game.Rounds.Last();
        var field = _helperService.ConvertFieldToArray(currentRound.Field);
        if (field[dto.Row, dto.Column] != Empty)
            return new BlErrorDto("PlaceNotEmpty", "Place for figure isn't empty");

        var currentUserPlaysCrosses = currentRound.InvitingUserPlaysCrosses
            ? currentUserId == game.InvitingUser.Id
            : currentUserId == game.InvitedUser.Id;
        var currentUserFigure = currentUserPlaysCrosses ? Cross : Nought;
        var timeToPlayCrosses = currentRound.Field.Count(c => c == Nought) == currentRound.Field.Count(c => c == Cross);
        if (currentUserPlaysCrosses != timeToPlayCrosses)
            return new BlErrorDto("NotYourMove", "This is your opponent's move");
        field[dto.Row, dto.Column] = currentUserFigure;
        currentRound.Field = _helperService.ConvertArrayToField(field);
        var result = _helperService.GetResult(field);
        if (result == GameResult.CrossesWon || result == GameResult.NoughtsWon || result == GameResult.Draw)
        {
            currentRound.Result = result;
            if (game.CountOfRounds == game.Rounds.Count)
            {
                var cntOfWinCrosses = game.Rounds.Count(r => r.Result == GameResult.CrossesWon);
                var cntOfWinNoughts = game.Rounds.Count(r => r.Result == GameResult.NoughtsWon);
                if (cntOfWinCrosses > cntOfWinNoughts) game.Result = GameResult.CrossesWon;
                else if (cntOfWinCrosses < cntOfWinNoughts) game.Result = GameResult.NoughtsWon;
                else game.Result = GameResult.Draw;
                game.Status = GameStatus.Ended;
            }
            else
            {
                game.Rounds.Add(new GameRound
                {
                    RoundNumber = currentRound.RoundNumber + 1,
                    Field = string.Join("", Enumerable.Repeat(' ', 9)),
                    InvitingUserPlaysCrosses = !currentRound.InvitingUserPlaysCrosses,
                    Result = GameResult.NotOver
                });
            }
        }

        await _db.SaveChangesAsync();
        return new Success();
    }

    public async Task<OneOf<Success, BlErrorDto>> CancelGame(CancelGameDto dto)
    {
        var currentUserId = _usersService.GetCurrentUserId();
        var game = await _db.Games.Where(g =>
                g.Id == dto.GameId &&
                (g.InvitingUser.Id == currentUserId || g.InvitedUser.Id == currentUserId) &&
                (g.Status == GameStatus.Invited || g.Status == GameStatus.Accepted))
            .FirstOrDefaultAsync();
        if (game == null)
            return new BlErrorDto("GameNotFound", "Game with this id doesn't exist");
        game.Status = GameStatus.Cancelled;
        await _db.SaveChangesAsync();
        return new Success();
    }
}