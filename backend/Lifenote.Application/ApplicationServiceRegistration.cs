using Lifenote.Application.Contracts;
using Lifenote.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lifenote.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IHabitService, HabitService>();
        services.AddScoped<INoteService, NoteService>();
        services.AddScoped<IHabitStreakService, HabitStreakService>();
        services.AddScoped<IUserInfoService, UserInfoService>();
        return services;
    }
}
