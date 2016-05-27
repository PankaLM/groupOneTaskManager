using System;
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
                .Where(t => t.UserId == userId && t.StateId != State.Done.StateId)
                .OrderByDescending(t => t.FlyScore)
                .AsEnumerable()
                .Select(t => new TaskVo()
                 {
                     TaskId = t.TaskId,
                     Deadline = t.Deadline,
                     Duration = t.Duration,
                     Title = t.Title,
                     FlyScore = t.FlyScore,
                     TooLate = t.LateStart.HasValue ? DateTime.Compare(t.LateStart.Value, DateTime.Now) <= 0 : false,
                     Tags = !string.IsNullOrEmpty(t.Tag) ? string.Join(", ", t.Tag.Split(TaskManagerConstants.Splitter.ToCharArray())) : ""
                 });
        }

        public IEnumerable<TaskMetricsVo> GetTaskMetrics(int userId)
        {
            return this.dbContextAccessor.DbContext.Set<TaskModel>()
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.FlyScore)
                .AsEnumerable()
                .Select(t =>new TaskMetricsVo()
                {
                    TaskId = t.TaskId,
                    Deadline = t.Deadline,
                    Duration = t.Duration,
                    Title = t.Title,
                    FlyScore = t.FlyScore,
                    Tags = !string.IsNullOrEmpty(t.Tag) ? string.Join(", ", t.Tag.Split(TaskManagerConstants.Splitter.ToCharArray())) : "",
                    CompletionTime = new EvaluatedTimeVo(t.CompletionTime),
                    ExecutionTime = new EvaluatedTimeVo(t.ExecutionTime),
                    WaitingTime = new EvaluatedTimeVo(t.WaitingTime)
                });
        }

        public DateTime? GetFirstOverloadedDay(int userId)
        {
            var day = this.dbContextAccessor.DbContext.Set<TaskModel>()
                .Where(t => t.UserId == userId && t.Deadline.HasValue && t.StateId != State.Done.StateId && t.Deadline >= DateTime.Now)
                .GroupBy(t => t.Deadline.Value)
                .Where(t => t.Count() >= 5)
                .OrderBy(d => d.Key)
                .Select(d => d.Key)
                .FirstOrDefault();

            return day.Equals(DateTime.MinValue) ? (DateTime?)null : day;
        }
    }
}
