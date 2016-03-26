using Autofac;
using TaskManager.Domain.Aggregates;
using TaskManager.Domain.Data.Common;

namespace TaskManager.Domain
{
    public class DomainModule : Module
    {
        protected override void Load(ContainerBuilder moduleBuilder)
        {
            moduleBuilder.RegisterType<DomainDbConfiguration>().As<IDbConfiguration>().SingleInstance();
        }
    }
}
