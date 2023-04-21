using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using MessengerApp.Domain.Contexts;
using MessengerApp.Domain.Entities;
using MessengerApp.Domain.Exceptions;
using MessengerApp.Domain.Models;
using MessengerApp.Services.Auth;
using MessengerApp.Services.Auth.Options;
using MessengerApp.Services.Extensions;
using MessengerApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MessengerApp.Services.Services;

public sealed class JwtService : IJwtService
{
    private MessengerAppContext MessengerAppContext { get; }
    private IPasswordHandler PasswordHandler { get; }
    private IMapper Mapper { get; }

    public JwtService(MessengerAppContext context, IPasswordHandler passwordHandler, IMapper mapper)
    {
        MessengerAppContext = context;
        PasswordHandler = passwordHandler;
        Mapper = mapper;
    }

    public async Task<string> Login(string email, string password, CancellationToken cancellationToken)
    {
        var identity = await GetIdentity(email, password, cancellationToken);
            
        if (identity is null)
        {
            throw new ArgumentException("Invalid username or password.");
        }
 
        var now = DateTime.UtcNow;
            
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.Issuer,
            audience: AuthOptions.Audience,
            notBefore: now,
            claims: identity.Claims,
            expires: now.Add(TimeSpan.FromMinutes(AuthOptions.Lifetime)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        if (encodedJwt is null)
        {
            throw new MessengerAppException();
        }
        
        return encodedJwt;
    }
    
    public async Task<UserDto> Register(UserDto newUserDto, CancellationToken cancellationToken)
    {
        ValidateRegisterRequest(newUserDto);

        var isExists = await MessengerAppContext.Users
            .Where(x => x.Email == newUserDto.Email)
            .AnyAsync(cancellationToken);

        if (isExists)
        {
            throw new MessengerAppException("User already exists");
        }
            
        PasswordHandler.CreateHash(newUserDto.Password!, out var hash, out var salt);

        var newEntity = new UserEntity
        {
            Email = newUserDto.Email!,
            PasswordHash = hash,
            PasswordSalt = salt,
            Nickname = newUserDto.Nickname!
        };

        MessengerAppContext.Users.Add(newEntity);
        await MessengerAppContext.SaveChangesAsync(cancellationToken);

        var result = Mapper.Map<UserDto>(newEntity);

        return result;
    }

    private async Task<ClaimsIdentity> GetIdentity(string email, string password, CancellationToken cancellationToken)
    {
        var user = await GetByEmailAndPassword(email, password, cancellationToken);
            
        var claims = new List<Claim>
        {
            new (ClaimsIdentity.DefaultNameClaimType, user.Email!),
        };
            
        var claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
        return claimsIdentity;
    }

    private async Task<UserDto> GetByEmailAndPassword(string email, string password, CancellationToken cancellationToken)
    {
        var user = await MessengerAppContext.Users
            .AsNoTracking()
            .Where(x => x.Email == email)
            .FirstOrNotFoundAsync(cancellationToken);

        var passwordIsValid = PasswordHandler.IsValid(password, user.PasswordHash, user.PasswordSalt);

        if (!passwordIsValid)
        {
            throw new Exception();
        }
            
        var result = Mapper.Map<UserDto>(user);

        return result;
    }

    private static void ValidateRegisterRequest(UserDto data)
    {
        if (string.IsNullOrWhiteSpace(data.Nickname)
            || string.IsNullOrWhiteSpace(data.Password))
        {
            throw new ArgumentException("Invalid user data");
        }
    }
}