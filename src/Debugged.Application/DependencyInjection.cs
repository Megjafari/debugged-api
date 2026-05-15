using System.Reflection;
using Debugged.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Debugged.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        services.AddAutoMapper(cfg => cfg.AddMaps(assembly));

        services.AddValidatorsFromAssembly(assembly);

        // ValidationBehavior must be registered AFTER MediatR — order matters
        // here, since MediatR's behavior pipeline runs in registration order.
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}