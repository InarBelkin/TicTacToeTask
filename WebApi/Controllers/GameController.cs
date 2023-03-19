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

    /// <summary>
    /// Invite user to a game.
    /// You can't invite a user that already has active game with you.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("invite")]
    public async Task<IActionResult> Invite(InviteModel model)
    {
        return (await _gameService.InviteUser(model)).Match<IActionResult>(
            s => Ok(),
            e => BadRequest(new ErrorsDto(e)));
    }

    /// <summary>
    /// Accept or decline invitation.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("inviteanswer")]
    public async Task<IActionResult> InviteAnswer(InviteAnswerModel model)
    {
        return (await _gameService.AnswerInvitation(model)).Match<IActionResult>(
            _ => Ok(),
            e => BadRequest(new ErrorsDto(e)));
    }

    /// <summary>
    /// Get all games of this user.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ICollection<GameItemDto>>> GetGames([FromQuery] GetGamesRequestDto dto)
    {
        return Ok(await _gameService.GetGames(dto));
    }

    /// <summary>
    /// Get detailed model of game
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpGet("one")]
    public async Task<IActionResult> GetOne([FromQuery] GetGameRequestDto dto)
    {
        return (await _gameService.GetGame(dto)).Match<IActionResult>(
            Ok,
            e => BadRequest(new ErrorsDto(e)));
    }

    /// <summary>
    /// Place your figure to a place.
    /// </summary>
    [HttpPatch("mark")]
    public async Task<IActionResult> PlaceMark(PlaceMarkDto dto)
    {
        return (await _gameService.PlaceMark(dto)).Match<IActionResult>(
            _ => Ok(),
            e => BadRequest(new ErrorsDto(e)));
    }

    /// <summary>
    /// Just cancel not ended battle.
    /// </summary>
    [HttpPatch("cancel")]
    public async Task<IActionResult> Cancel(CancelGameDto dto)
    {
        return (await _gameService.CancelGame(dto)).Match<IActionResult>(
            _ => Ok(),
            e => BadRequest(new ErrorsDto(e)));
    }
}