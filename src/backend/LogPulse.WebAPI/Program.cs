using LogPulse.WebAPI.BackgroundServices;
using LogPulse.WebAPI.Context;
using LogPulse.WebAPI.Helpers;
using LogPulse.WebAPI.Hubs;
using LogPulse.WebAPI.Interfaces;
using LogPulse.WebAPI.Middlewares;
using LogPulse.WebAPI.Models.Settings;
using LogPulse.WebAPI.Repositories;
using LogPulse.WebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

builder.Services.AddOptions<DbSettings>().BindConfiguration("Db").ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<ClientSettings>().BindConfiguration("Client").ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<DemoUserSettings>().BindConfiguration("DemoUser").ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<JwtSettings>().BindConfiguration("Jwt").ValidateDataAnnotations().ValidateOnStart();

builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<LogSimulatorWorker>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<LogSimulatorWorker>());
builder.Services.AddHostedService<DatabaseHostedService>();
builder.Services.AddHostedService<DatabaseCleaningWorker>();

builder.Services.AddControllers();

builder.Services.AddSignalR();

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ?? throw new InvalidOperationException("JwtSettings configuration is missing.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var rsaKey = RsaKeyLoader.LoadPublicKey(jwtSettings.PublicKeyPath);
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = rsaKey,

            NameClaimType = JwtRegisteredClaimNames.Email
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var path = context.HttpContext.Request.Path;
                if (path.StartsWithSegments("/logHub"))
                {
                    var accessToken = context.Request.Query["access_token"];

                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                    }
                }
                return Task.CompletedTask;
            }
        };
    });

var clientSettings = builder.Configuration.GetSection("Client").Get<ClientSettings>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        if (clientSettings != null && !string.IsNullOrEmpty(clientSettings.BaseUrl))
        {
            policy.WithOrigins(clientSettings.BaseUrl)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseMiddleware<RequestMetadataMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();

app.UseAuthorization();

app.MapHub<LogHub>("/logHub");
app.MapControllers();

app.Run();