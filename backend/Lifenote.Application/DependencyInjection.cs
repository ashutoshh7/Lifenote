using Lifenote.Application.Contracts;
using Lifenote.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lifenote.Application;

/// <summary>
/// Single DI registration entrypoint for the Application layer.
/// Call builder.Services.AddApplication() in Program.cs.
///
/// Note: ApplicationServiceRegistration.cs has been deleted — this file is
/// the sole registration point. Do not re-introduce a second DI helper.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<INoteService, NoteService>();
        services.AddScoped<IUserInfoService, UserInfoService>();
        return services;
    }
}
