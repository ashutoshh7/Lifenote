using Lifenote.Application.Contracts;
using Lifenote.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lifenote.Application;

/// <summary>
/// Application layer DI extension — registers use case / service classes.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IHabitService, HabitService>();
        services.AddScoped<IHabitStreakService, HabitStreakService>();
        services.AddScoped<INoteService, NoteService>();
        services.AddScoped<IUserInfoService, UserInfoService>();
        return services;
    }
}
