using Lifenote.Application.Contracts;
using Lifenote.Infrastructure.Persistence;
using Lifenote.Infrastructure.Repositories;
using Lifenote.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lifenote.Infrastructure;

/// <summary>
/// DI extension method keeps Program.cs clean.
/// All Infrastructure-level registrations live here.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<LifenoteDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IHabitRepository, HabitRepository>();
        services.AddScoped<INoteRepository, NoteRepository>();
        services.AddScoped<IUserInfoRepository, UserInfoRepository>();
        services.AddScoped<IHabitStreakRepository, HabitStreakRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Infrastructure Services (depend on HttpContext or external SDKs)
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IFirebaseClaimService, FirebaseClaimService>();

        return services;
    }
}
