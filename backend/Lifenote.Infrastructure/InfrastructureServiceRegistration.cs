using Lifenote.Application.Contracts;
using Lifenote.Core.Interfaces;
using Lifenote.Infrastructure.Persistence;
using Lifenote.Infrastructure.Persistence.Repositories;
using Lifenote.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lifenote.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<LifenoteDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IUserInfoRepository, UserInfoRepository>();
        services.AddScoped<INoteRepository, NoteRepository>();
        services.AddScoped<IHabitRepository, HabitRepository>();
        services.AddScoped<IHabitStreakRepository, HabitStreakRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Infrastructure services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IFirebaseClaimService, FirebaseClaimService>();

        return services;
    }
}
