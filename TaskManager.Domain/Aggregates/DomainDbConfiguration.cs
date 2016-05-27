using System.Data.Entity;
using TaskManager.Domain.Aggregates.Tasks;
using TaskManager.Domain.Aggregates.Users;
using TaskManager.Domain.Data.Common;

namespace TaskManager.Domain.Aggregates
{
    public class DomainDbConfiguration : IDbConfiguration
    {
        public void AddConfiguration(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AchievementMap());
            modelBuilder.Configurations.Add(new TaskMap());
            modelBuilder.Configurations.Add(new UserAchievementMap());
            modelBuilder.Configurations.Add(new UserMap());
        }
    }
}
