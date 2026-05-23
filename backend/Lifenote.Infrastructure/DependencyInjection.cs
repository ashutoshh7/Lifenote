using Lifenote.Application.Contracts;
using Lifenote.Domain.Interfaces;
using Lifenote.Infrastructure.Persistence;
using Lifenote.Infrastructure.Repositories;
using Lifenote.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lifenote.Infrastructure;

/// <summary>
/// Single DI entry-point for all Infrastructure registrations.
/// Repository + UoW interfaces live in Lifenote.Domain.Interfaces.
/// Service interfaces (ICurrentUserService, IFirebaseClaimService) live in Lifenote.Application.Contracts.
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

        // Repositories — interfaces from Domain.Interfaces, implementations from Infrastructure.Repositories
        services.AddScoped<IHabitRepository, HabitRepository>();
        services.AddScoped<INoteRepository, NoteRepository>();
        services.AddScoped<IUserInfoRepository, UserInfoRepository>();
        services.AddScoped<IHabitStreakRepository, HabitStreakRepository>();

        // Unit of Work — interface from Domain.Interfaces
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Infrastructure Services — interfaces from Application.Contracts
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IFirebaseClaimService, FirebaseClaimService>();

        return services;
    }
}
