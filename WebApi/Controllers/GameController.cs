using Core.Dtos;
using Core.Model;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    private readonly GameService _gameService;

    public GameController(GameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost("invite")]
    public async Task<IActionResult> Invite(InviteModel model)
    {
        return (await _gameService.InviteUser(model)).Match<IActionResult>(
            s => Ok(),
            e => BadRequest(new ErrorsDto(e)));
    }

    [HttpPost("inviteanswer")]
    public async Task<IActionResult> InviteAnswer(InviteAnswerModel model)
    {
        return (await _gameService.AnswerInvitation(model)).Match<IActionResult>(
            _ => Ok(),
            e => BadRequest(new ErrorsDto(e)));
    }

    [HttpGet]
    public async Task<ActionResult<ICollection<GameItemDto>>> GetGames([FromQuery] GetGamesRequestDto dto)
    {
        return Ok(await _gameService.GetGames(dto));
    }

    [HttpGet("one")]
    public async Task<IActionResult> GetOne([FromQuery] GetGameRequestDto dto)
    {
        return (await _gameService.GetGame(dto)).Match<IActionResult>(
            Ok,
            e => BadRequest(new ErrorsDto(e)));
    }

    [HttpPatch("mark")]
    public async Task<IActionResult> PlaceMark(PlaceMarkDto dto)
    {
        return (await _gameService.PlaceMark(dto)).Match<IActionResult>(
            _ => Ok(),
            e => BadRequest(new ErrorsDto(e)));
    }

    [HttpPatch("cancel")]
    public async Task<IActionResult> Cancel(CancelGameDto dto)
    {
        return (await _gameService.CancelGame(dto)).Match<IActionResult>(
            _ => Ok(),
            e => BadRequest(new ErrorsDto(e)));
    }
}