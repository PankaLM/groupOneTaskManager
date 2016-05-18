using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TaskManager.Common;
using TaskManager.Data.Common;
using TaskManager.Domain.Aggregates.Tasks;
using TaskManager.Domain.Data.Common;
using TaskManager.Domain.Data.Repositories.Tasks;
using TaskManager.Domain.Data.ViewObjects;

namespace TaskManager.Data.Repositories
{
    internal class TasksRepository : AggregateRepository<TaskModel>, ITasksRepository
    {
        public TasksRepository(IDbContextAccessor dbContextAccessor)
            : base(dbContextAccessor)
        {
        }

        public override TaskModel Find(int id)
        {
            return this.dbContextAccessor.DbContext.Set<TaskModel>()
                .Include(t => t.RecurringTaskGroup)
                .Where(t => t.TaskId == id)
                .SingleOrDefault();
        }

        public IEnumerable<TaskVo> GetTasks(int userId)
        {
            return this.dbContextAccessor.DbContext.Set<TaskModel>()
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.FlyScore)
                .AsEnumerable()
                .Select(t => new TaskVo()
                 {
                     TaskId = t.TaskId,
                     Deadline = t.Deadline,
                     Duration = t.Duration,
                     Title = t.Title,
                     FlyScore = t.FlyScore,
                     Tags = !string.IsNullOrEmpty(t.Tag) ? string.Join(", ", t.Tag.Split(TaskManagerConstants.Splitter.ToCharArray())) : ""
                 });
        }
    }
}
