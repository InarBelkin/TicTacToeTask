using Core.Entities.Enums;

namespace Core.Dtos;

public class GameRoundDto
{
    public int RoundNumber { get; set; }
    public required string Field { get; set; }
    public required bool InvitingUserPlaysCrosses { get; set; }
    public required GameResult Result { get; set; }
}