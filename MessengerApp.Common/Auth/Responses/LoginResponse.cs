namespace MessengerApp.Common.Auth.Responses;

public class LoginResponse
{
    public string Token { get; set; } = null!;
    public string UserName { get; set; } = null!;
}