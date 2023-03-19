using Core.Entities;
using Core.Model;
using Microsoft.AspNetCore.Identity;

namespace Core.Services;

public interface IUsersService
{
    Task<RegisterResult> Register(RegisterModel model);
    Task<LoginResult> Login(LoginModel model);
    int GetCurrentUserId();
}

public class UsersService : IUsersService
{
    private readonly HttpContext _context;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public UsersService(UserManager<User> userManager, SignInManager<User> signInManager, IHttpContextAccessor accessor)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = accessor.HttpContext!;
    }

    public async Task<RegisterResult> Register(RegisterModel model)
    {
        var user = new User { UserName = model.UserName };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, false);
            return new RegisterResult { Message = "Registration is successful" };
        }

        return new RegisterResult
            { Message = "Registration error:\n" + string.Join("\n", result.Errors.Select(e => e.Description)) };
    }

    public async Task<LoginResult> Login(LoginModel model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
        if (result.Succeeded)
            return new LoginResult { Message = "Login is successful" };
        return new LoginResult { Message = "Login errror" };
    }

    public int GetCurrentUserId()
    {
        return int.Parse(_userManager.GetUserId(_context.User)!);
    }
}