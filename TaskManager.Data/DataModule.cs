using Autofac;
using TaskManager.Data.Common;
using TaskManager.Data.Repositories;
using TaskManager.Domain.Data.Common;
using TaskManager.Domain.Data.Repositories.Tasks;
using TaskManager.Domain.Data.Repositories.Users;
using System.Net.Http;

namespace TaskManager.Data
{
    public class DataModule : Module
    {
        protected override void Load(ContainerBuilder moduleBuilder)
        {
            moduleBuilder.RegisterType<UnitOfWork>().As<IUnitOfWork>().As<IDbContextAccessor>().InstancePerLifetimeScope();
            moduleBuilder.Register(c => c.Resolve<HttpRequestMessage>().GetUserContext()).InstancePerRequest();

            moduleBuilder.RegisterType<UsersRepository>().As<IUsersRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<TasksRepository>().As<ITasksRepository>().InstancePerLifetimeScope();
        }
    }
}
