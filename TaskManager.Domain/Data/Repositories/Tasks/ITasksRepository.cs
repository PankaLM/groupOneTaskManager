using System;
using System.Collections.Generic;
using TaskManager.Domain.Aggregates.Tasks;
using TaskManager.Domain.Data.Common;
using TaskManager.Domain.Data.ViewObjects;

namespace TaskManager.Domain.Data.Repositories.Tasks
{
    public interface ITasksRepository : IAggregateRepository<TaskModel>
    {
        IEnumerable<TaskVo> GetTasks(int userId);

        IEnumerable<RecurringTaskGroupTaskVo> GetRecurringTaskGroups(int userId);

        IEnumerable<TaskMetricsVo> GetTaskMetrics(int userId);

        DateTime? GetFirstOverloadedDay(int userId);
    }
}
