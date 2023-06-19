using System.Reflection;
using Autofac;
using EventBus.Abstractions;
using Identity.Api.Application.DomainEventHandlers.Users;
using Identity.Api.Infrastructure.Services;
using Identity.Domain.Aggregates.Users;
using Identity.Domain.IServices;
using Identity.Domain.Validations.Users;
using Identity.Infrastructure.BcValidations;
using Identity.Infrastructure.Dapper;
using Identity.Infrastructure.EF.Stores;
using IdentityService.Utils;
using MediatR;

namespace Identity.Api.Infrastructure.AutofacModules;

public class ApplicationModule
    : Autofac.Module
{
    private readonly string dapperConnectionString;

    public ApplicationModule(string dapperConnectionString)
    {
        this.dapperConnectionString = dapperConnectionString;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<JwtTokenGeneratorService>().As<ITokenGeneratorService>().InstancePerLifetimeScope();
        builder.RegisterType<PasswordService>().As<IPasswordService>().InstancePerLifetimeScope();
        builder.RegisterType<AppRandoms>().As<IAppRandoms>().InstancePerLifetimeScope();
        builder.RegisterType<Brokers.EventBus>().As<IEventBus>().InstancePerLifetimeScope();
        builder.RegisterType<DapperQueryExecutor>().As<IQueryExecutor>().InstancePerLifetimeScope();
        builder.RegisterType<CurrentUser>().As<ICurrentUser>().InstancePerLifetimeScope();
        builder.RegisterType<DapperContext>().WithParameter(new TypedParameter(typeof(string), dapperConnectionString));
        
        builder.RegisterAssemblyTypes(
                Assembly.GetAssembly(typeof(IUserStore))!, Assembly.GetAssembly(typeof(UserStore))!)
            .Where(t => t.Name.EndsWith("Store"))
            .AsImplementedInterfaces().InstancePerLifetimeScope();
        
        //Inject bounded context validations
        builder.RegisterAssemblyTypes(
                Assembly.GetAssembly(typeof(IUserBcScopeValidation))!, Assembly.GetAssembly(typeof(UserBcScopeValidation))!)
            .Where(t => t.Name.EndsWith("Validation"))
            .AsImplementedInterfaces().InstancePerLifetimeScope();
        
        // Register the DomainEventHandler classes (they implement INotificationHandler<>) in assembly holding the Domain Events
        builder.RegisterAssemblyTypes(typeof(TestDomainEventHandler).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(INotificationHandler<>));
    }
}
