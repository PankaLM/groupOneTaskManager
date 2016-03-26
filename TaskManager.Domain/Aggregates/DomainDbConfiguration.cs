using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Aggregates.Users;
using TaskManager.Domain.Data.Common;

namespace TaskManager.Domain.Aggregates
{
    public class DomainDbConfiguration : IDbConfiguration
    {
        public void AddConfiguration(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserMap());
        }
    }
}
