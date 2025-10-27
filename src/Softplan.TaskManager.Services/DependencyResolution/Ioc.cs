using Microsoft.Extensions.DependencyInjection;

namespace Softplan.TaskManager.Services.DependencyResolution;

public static class Ioc
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenJwtService>();
    }
}