using System.Reflection;
using Autofac;
using Identity.Api.Infrastructure.AppServices;
using IdentityService.Api.Application.Commands.Users;
using IdentityService.Data.Stores.Users;

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
        //TODO: change this and use assembly 
        builder.RegisterType<UserStore>().As<IUserStore>().InstancePerLifetimeScope();
        builder.RegisterType<IPasswordService>().As<PasswordService>().InstancePerLifetimeScope();

        // builder.RegisterAssemblyTypes(typeof(AddUserCommand).GetTypeInfo().Assembly)
        //     .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
    }
}
