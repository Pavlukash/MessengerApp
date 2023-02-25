using AutoMapper;
using MessengerApp.Services.Interfaces;
using MessengerApp.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.IdentityModel.Tokens;
using MessengerApp.Domain.Contexts;
using MessengerApp.Hubs;
using MessengerApp.Middleware;
using MessengerApp.Services.Auth;
using MessengerApp.Services.Auth.Options;
using MessengerApp.Services.Mapping;
using Microsoft.EntityFrameworkCore;

using var host = Host.CreateDefaultBuilder(args).Build();

var builder = WebApplication.CreateBuilder(args);

var pwdOptions = new PwdOptions();
builder.Configuration.Bind("pwdOptions", pwdOptions);
builder.Services.AddSingleton(pwdOptions);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        { 
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.Issuer, 
            ValidateAudience = true,
            ValidAudience = AuthOptions.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };
    });
builder.Services.AddControllers();
            
var connection = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<MessengerAppContext>(options =>
    options.UseSqlServer(connection, b => b.MigrationsAssembly("MessengerApp")));

builder.Services.AddSignalR();

var mapperConfig = new MapperConfiguration(x =>
{
    x.AddProfile(new EntityToDtoProfile());
    x.AddProfile(new DtoToEntityProfile());
});

var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddCors(x => x.AddPolicy("MyPolicy", policyBuilder =>
{
    policyBuilder
        .WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader();
}));

builder.Services.AddScoped<IPasswordHandler, PasswordHandler>();
builder.Services.AddScoped<IJwtService, JwtService>();


var app = builder.Build();

app.UseCors("MyPolicy");

app.UseHttpsRedirection();

app.UseRouting();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
            
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    
    app.MapHub<ChatHub>("/chat",
        options => {
            options.ApplicationMaxBufferSize = 128;
            options.TransportMaxBufferSize = 128;
            options.LongPolling.PollTimeout = TimeSpan.FromMinutes(1);
            options.Transports = HttpTransportType.LongPolling | HttpTransportType.WebSockets;
        });
});
 
app.Run();