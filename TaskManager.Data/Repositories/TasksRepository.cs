using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Data.Common;
using TaskManager.Domain.Data.Repositories.Tasks;
using TaskManager.Domain.Data.ViewObjects;

namespace TaskManager.Data.Repositories
{
    public class TasksRepository : ITasksRepository
    {
        private IDbContextAccessor dbContextAccessor;
        public TasksRepository(IDbContextAccessor dbContextAccessor)
        {
            this.dbContextAccessor = dbContextAccessor;
        }

        public IEnumerable<TaskVo> GetTasks(int userId)
        {
            return null;//this.dbContextAccessor.DbContext.Set<Task>();
        }
    }
}
