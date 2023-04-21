namespace MessengerApp.Domain.Models;

public sealed class UserDto
{
    public int Id { get; set; } 
    public string? Nickname { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}