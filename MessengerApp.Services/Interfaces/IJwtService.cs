using MessengerApp.Common.Auth.Responses;
using MessengerApp.Domain.Models;

namespace MessengerApp.Services.Interfaces;

public interface IJwtService
{
    Task<LoginResponse> Login(string username, string password, CancellationToken cancellationToken);
    Task<UserDto> Register(UserDto newClientEntity, CancellationToken cancellationToken);
}