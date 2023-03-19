namespace Core.Model;

public class LoginModel
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required bool RememberMe { get; set; }
}