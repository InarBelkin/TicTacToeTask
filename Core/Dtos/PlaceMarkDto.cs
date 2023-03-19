using System.ComponentModel.DataAnnotations;

namespace Core.Dtos;

public class PlaceMarkDto
{
    public required int GameId { get; init; }
    [Range(0, 2)] public required int Row { get; init; }
    [Range(0, 2)] public required int Column { get; init; }
}