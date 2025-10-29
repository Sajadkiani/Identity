using System.Reflection;
using Identity.Api.Application.Behaviors;
using Identity.Api.Application.Commands.Users;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Api.AutofacModules;

public static class MediatorConfiguration
{
    public static IServiceCollection AddMediatorModule(this IServiceCollection services)
    {
        var applicationAssembly = typeof(AddUserCommandHandler).GetTypeInfo().Assembly;

        // Registers all IRequestHandler, INotificationHandler, etc.
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(applicationAssembly);
        });

        // Register pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));

        return services;
    }
}
