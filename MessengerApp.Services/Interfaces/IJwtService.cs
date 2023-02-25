using MessengerApp.Domain.Models;
using MessengerApp.Services.Auth.Responses;

namespace MessengerApp.Services.Interfaces;

public interface IJwtService
{
    Task<string> Login(string username, string password, CancellationToken cancellationToken);
    Task<UserDto> Register(UserDto newClientEntity, CancellationToken cancellationToken);
}