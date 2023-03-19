using Core.Entities.Enums;

namespace Core.Entities;

public class Game
{
    public int Id { get; set; }

    public required User InvitingUser { get; set; }

    public required User InvitedUser { get; set; }
    public ICollection<GameRound> Rounds { get; set; } = new List<GameRound>();


    public required GameStatus Status { get; set; }
    public required int CountOfRounds { get; set; }
    public required bool InvitingUserPlaysFirst { get; set; }
    public required GameResult Result { get; set; }
}