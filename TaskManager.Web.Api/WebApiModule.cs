using Autofac;
using Microsoft.Owin.Security.OAuth;
using TaskManager.Common.Jobs;
using TaskManager.Web.Api.Controllers;
using TaskManager.Web.Api.Jobs;
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
            moduleBuilder.RegisterType<TaskNomsController>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<StateNomsController>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<ActionNomsController>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<UsersController>().InstancePerLifetimeScope();

            moduleBuilder.RegisterType<EmailSender>().As<IJob>().ExternallyOwned();
            moduleBuilder.RegisterType<RecurringTasksCreator>().As<IJob>().ExternallyOwned();
        }
    }
}
