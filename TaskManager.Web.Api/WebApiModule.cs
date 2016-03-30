using Autofac;
using Microsoft.Owin.Security.OAuth;
using TaskManager.Web.Api.Controllers;
using TaskManager.Web.Api.OAuth;

namespace TaskManager.Web.Api
{
    public class WebApiModule : Module
    {
        protected override void Load(ContainerBuilder moduleBuilder)
        {

            moduleBuilder.RegisterType<ApplicationOAuthServerProvider>().As<IOAuthAuthorizationServerProvider>().SingleInstance();
            moduleBuilder.RegisterType<ApplicationOAuthBearerProvider>().As<IOAuthBearerAuthenticationProvider>().SingleInstance();
            
            moduleBuilder.RegisterType<TasksController>().InstancePerLifetimeScope();

        }
}
}
