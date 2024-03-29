﻿using System.Reflection;
using Autofac;
using EventBus.Abstractions;
using Identity.Api.Application.DomainEventHandlers.Users;
using Identity.Api.Infrastructure.Services;
using Identity.Domain.Aggregates.Users;
using Identity.Domain.IServices;
using Identity.Domain.Validations.Users;
using Identity.Infrastructure.Dapper;
using Identity.Infrastructure.EF.Stores;
using Identity.Infrastructure.EF.Utils;
using Identity.Infrastructure.ORM.BcValidations;
using Identity.Infrastructure.Utils;
using IntegrationEventLogEF.Services;
using MediatR;

namespace Identity.Api.AutofacModules;

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

        //TODO: add circuit breaker and retry pattern if it will need AddHttpClient
        //https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-circuit-breaker-pattern
        
        builder.RegisterType<PasswordService>().As<IPasswordService>().InstancePerLifetimeScope();
        builder.RegisterType<EventInitializer>().As<IEventInitializer>().InstancePerLifetimeScope();
        builder.RegisterType<AppRandoms>().As<IAppRandoms>().InstancePerLifetimeScope();
        builder.RegisterType<Identity.Infrastructure.Brokers.EventBus>().As<IEventBus>().InstancePerLifetimeScope();
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
            .AsClosedTypesOf(typeof(INotificationHandler<>)).InstancePerLifetimeScope();
    }
}
