using System.ComponentModel.DataAnnotations;

namespace Core.Dtos;

public class GetGameRequestDto
{
    [Required] public int GameId { get; set; }
}