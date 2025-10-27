using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Softplan.TaskManager.Dominio.Validators;

namespace Softplan.TaskManager.Dominio.DependencyResolution;

public static class Ioc
{
    public static void AddDomain(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<NewUserDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<NewTaskDtoValidator>();
    }
}