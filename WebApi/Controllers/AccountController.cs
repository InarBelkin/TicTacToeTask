using Core.Entities;
using Core.Model;
using Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<User> _manager;
    private readonly IUsersService _usersService;

    public AccountController(IUsersService usersService, UserManager<User> manager)
    {
        _usersService = usersService;
        _manager = manager;
    }

    /// <summary>
    /// Registration
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResult>> Register(RegisterModel registerModel)
    {
        var result = await _usersService.Register(registerModel);
        return Ok(result);
    }

    /// <summary>
    /// Login
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var result = await _usersService.Login(model);
        return Ok(result);
    }
}