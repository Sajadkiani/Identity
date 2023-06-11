using System.Reflection;
using Autofac;
using Identity.Api.Application.Commands.Users;
using Identity.Api.Infrastructure.AppServices;
using Identity.Api.Infrastructure.Brokers;
using Identity.Domain.Aggregates.Users;
using Identity.Domain.SeedWork;
using Identity.Infrastructure.Dapper;
using Identity.Infrastructure.EF.Stores;
using IdentityService.Utils;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Infrastructure.AutofacModules;

public class ApplicationModule
    : Autofac.Module
{
    public string QueriesConnectionString { get; }

    public ApplicationModule(string qconstr)
    {
        QueriesConnectionString = qconstr;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<UserStore>().As<IUserStore>().InstancePerLifetimeScope();
        builder.RegisterType<IPasswordService>().As<PasswordService>().InstancePerLifetimeScope();
        builder.RegisterType<ITokenGeneratorService>().As<JwtTokenGeneratorService>().InstancePerLifetimeScope();
        builder.RegisterType<IAppRandoms>().As<AppRandoms>().InstancePerLifetimeScope();
        builder.RegisterType<IEventHandler>().As<EventHandler>().InstancePerLifetimeScope();
        builder.RegisterType<IQueryExecutor>().As<DapperQueryExecutor>().InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(
                Assembly.GetAssembly(typeof(IUserStore))!, Assembly.GetAssembly(typeof(UserStore))!)
            .Where(t => t.Name.EndsWith("Store"))
            .AsImplementedInterfaces().InstancePerLifetimeScope();
    }
}
