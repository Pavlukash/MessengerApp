using AutoMapper;
using MessengerApp.Domain.Contexts;
using MessengerApp.Middleware;
using MessengerApp.Services.Auth;
using MessengerApp.Services.Auth.Options;
using MessengerApp.Services.Interfaces;
using MessengerApp.Services.Mapping;
using MessengerApp.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace MessengerApp;

public static class Startup
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        
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
        
        var pwdOptions = new PwdOptions();
        builder.Configuration.Bind("pwdOptions", pwdOptions);
        builder.Services.AddSingleton(pwdOptions);
        
        builder.Services.AddDbContext<MessengerAppContext>(opt =>
        {
            opt.UseNpgsql(builder.Configuration.GetConnectionString("Default"), 
                x => x.MigrationsAssembly("MessengerApp"));
        });
        
        builder.Services.AddSwaggerGen(x =>
        {
            x.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "MessengerApp",
                    Version = "v1"
                });

            x.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT without Bearer into field",
                    Name = "Authorization",
                    BearerFormat = "JWT",
                    Scheme = "Bearer",
                    Type = SecuritySchemeType.Http
                });

            x.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        
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
        
        builder.AddServices();
    }

    public static void SetupPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
    }

    private static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IPasswordHandler, PasswordHandler>();
        builder.Services.AddScoped<IJwtService, JwtService>();
    }
}