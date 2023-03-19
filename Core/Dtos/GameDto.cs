using Core.Entities.Enums;

namespace Core.Dtos;

public class GameDto
{
    public required int Id { get; set; }
    public required int InvitingUserId { get; set; }
    public required int InvitedUserId { get; set; }
    public required int CountOfRounds { get; set; }

    public required List<GameRoundDto> Rounds { get; set; } = new();
    public required bool InvitingUserPlaysFirst { get; set; }
    public required GameStatus Status { get; set; }

    public required GameResult Result { get; set; }
}