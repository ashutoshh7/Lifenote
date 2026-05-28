using Lifenote.API.Middleware;
using Lifenote.Application;
using Lifenote.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.HttpOverrides;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
var projectId = builder.Configuration["Firebase:ProjectId"];

// ── Firebase Admin SDK (optional — for custom claims) ──────────────────────
var credentialsPath = builder.Configuration["Firebase:CredentialsPath"];
if (!string.IsNullOrWhiteSpace(credentialsPath) && File.Exists(credentialsPath))
{
    try
    {
        FirebaseAdmin.FirebaseApp.Create(new FirebaseAdmin.AppOptions
        {
            Credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(credentialsPath)
        });
    }
    catch { /* Custom claims skipped — graceful degradation */ }
}

// ── CORS ───────────────────────────────────────────────────────────────────
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:4200" };
builder.Services.AddCors(options =>
    options.AddPolicy("AllowAngularApp",
        policy => policy
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()));

// ── Rate Limiting & Forwarded Headers ──────────────────────────────────────
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("GlobalPolicy", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 100;
        opt.QueueLimit = 0;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Clear known networks/proxies to trust all if behind Railway/Azure
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

// ── Authentication (Firebase JWT) ──────────────────────────────────────────
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://securetoken.google.com/{projectId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{projectId}",
            ValidateAudience = true,
            ValidAudience = projectId,
            ValidateLifetime = true
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(context.Exception, "Authentication failed. Exception: {Message}", context.Exception.Message);
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogWarning("Authentication challenge triggered. Error: {Error}, Description: {ErrorDescription}", context.Error, context.ErrorDescription);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

// ── Clean Architecture DI Registration ─────────────────────────────────────
// Application layer services (use cases, business logic)
builder.Services.AddApplication();
// Infrastructure layer (DbContext, repositories, Firebase, CurrentUserService)
builder.Services.AddInfrastructure(builder.Configuration);

// ── API Layer ──────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Lifenote API v1",
        Version = "v1"
    }));

var app = builder.Build();

// ── Middleware Pipeline ────────────────────────────────────────────────────
// Global exception handler MUST be first
app.UseForwardedHeaders();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireRateLimiting("GlobalPolicy");

app.Run();
