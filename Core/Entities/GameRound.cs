using System.ComponentModel.DataAnnotations;
using Core.Entities.Enums;

namespace Core.Entities;

public class GameRound
{
    public int Id { get; set; }
    public int RoundNumber { get; set; }
    [Required] public Game? Game { get; set; }
    public required string Field { get; set; } = string.Join("", Enumerable.Repeat(' ', 9));
    public required bool InvitingUserPlaysCrosses { get; set; }
    public required GameResult Result { get; set; }
}