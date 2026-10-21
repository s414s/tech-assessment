namespace BookingSystem.Application.Handlers.Users;

public sealed record LoginQuery(string Email, string Password);
public sealed record LoginResponse(string Token);

public class LoginHandler
{

}
