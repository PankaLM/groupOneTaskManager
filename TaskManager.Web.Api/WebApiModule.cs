using Autofac;
using Microsoft.Owin.Security.OAuth;
using TaskManager.Web.Api.Controllers;
using TaskManager.Web.Api.OAuth;
using TaskManager.Web.Api.Tasks.Controllers;
using TaskManager.Web.Api.Users;

namespace TaskManager.Web.Api
{
    public class WebApiModule : Module
    {
        protected override void Load(ContainerBuilder moduleBuilder)
        {
            moduleBuilder.RegisterType<ApplicationOAuthServerProvider>().As<IOAuthAuthorizationServerProvider>().SingleInstance();
            moduleBuilder.RegisterType<ApplicationOAuthBearerProvider>().As<IOAuthBearerAuthenticationProvider>().SingleInstance();
            
            moduleBuilder.RegisterType<TasksController>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<StateNomsController>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<UsersController>().InstancePerLifetimeScope();
        }
    }
}
