using MessengerApp.Domain.Models;
using MessengerApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MessengerApp.Controllers;

public sealed class JwtController : ControllerBase
{
    private IJwtService JwtService { get; }
        
    public JwtController(IJwtService jwtService)
    {
        JwtService = jwtService;
    }

    [HttpPost("Auth/Login")]
    public async Task<IActionResult> Login(string email, string password, CancellationToken cancellationToken)
    {
        var result = await JwtService.Login(email, password, cancellationToken);

        return Ok(result);
    }

    [HttpPost("Auth/Register")]
    public async Task<IActionResult> Register([FromBody]UserDto newUserDto, CancellationToken cancellationToken)
    {
        var result = await JwtService.Register(newUserDto, cancellationToken);

        return Ok(result);
    }
}