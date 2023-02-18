namespace MessengerApp.Domain.Models;

public class UserDto
{
    public int Id { get; set; } 
    public string? Nickname { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? PhoneNumber { get; set; } 
}