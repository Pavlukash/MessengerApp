using MessengerApp.Common.Auth.Options;
using MessengerApp.Services.Interfaces;
using MessengerApp.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.IdentityModel.Tokens;
using MessengerApp.Domain.Contexts;
using Microsoft.EntityFrameworkCore;
using SignalRTraining.Hubs;

using var host = Host.CreateDefaultBuilder(args).Build();

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = host.Services.GetRequiredService<IConfiguration>();

IServiceCollection services = new ServiceCollection();

services.AddSignalR();
services.AddScoped<IJwtService, JwtService>();

var pwdOptions = new PwdOptions();
configuration.Bind("pwdOptions", pwdOptions);
services.AddSingleton(pwdOptions);

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

services.AddControllers();
            
var connection = configuration.GetConnectionString("Default");
services.AddDbContext<MessengerAppContext>(options =>
    options.UseSqlServer(connection, b => b.MigrationsAssembly("MessengerApp")));

var app = builder.Build();

app.UseDefaultFiles();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseRouting();

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