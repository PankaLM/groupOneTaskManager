using System;
using System.Collections.Generic;
using System.Web.Http;
using TaskManager.Common;
using TaskManager.Common.DataObjects;
using TaskManager.Common.UserContextModels;
using TaskManager.Domain.Aggregates.Tasks;
using TaskManager.Domain.Data.Common;
using TaskManager.Domain.Data.Repositories.Tasks;
using TaskManager.Domain.Data.ViewObjects;
using TaskManager.Web.Api.Tasks.DataObjects;
using TaskManager.Web.Api.Utils;
using System.Linq;
using System.Data.Entity;

namespace TaskManager.Web.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/tasks")]
    public class TasksController : ApiController
    {
        private UserContext userContext;
        private IUnitOfWork unitOfWork;
        private ITasksRepository tasksRepository;
        private IDbContextAccessor dbContextAccessor;

        public TasksController(
            UserContext userContext,
            IUnitOfWork unitOfWork,
            ITasksRepository tasksRepository,
            IDbContextAccessor dbContextAccessor)
        {
            this.userContext = userContext;
            this.unitOfWork = unitOfWork;
            this.tasksRepository = tasksRepository;
            this.dbContextAccessor = dbContextAccessor;
        }

        [Route("")]
        [HttpGet]
        public IEnumerable<TaskVo> GetTasks()
        {
            return this.tasksRepository.GetTasks(this.userContext.UserId);
        }

        [Route("metrics")]
        [HttpGet]
        public IEnumerable<TaskMetricsVo> GetTaskМetrics()
        {
            return this.tasksRepository.GetTaskMetrics(this.userContext.UserId);
        }

        [Route("new")]
        [HttpGet]
        public TaskDo GetNewTask()
        {
            return new TaskDo()
            {
                StateId = State.Initialized.StateId
            };
        }

        [Route("")]
        [HttpPost]
        [Transaction]
        public CreateResultDo CreateTaskDo(TaskDo taskDo)
        {
            var dependantTask = taskDo.DependantTaskId.HasValue ? this.tasksRepository.Find(taskDo.DependantTaskId.Value) : null;
            TaskModel task = new TaskModel(
                this.userContext,
                taskDo.InternalImportance,
                taskDo.ExternalImportance,
                taskDo.Clearness,
                taskDo.Closeness,
                taskDo.Simplicity,
                taskDo.Title,
                taskDo.Description,
                taskDo.Tag,
                taskDo.Thumbnail,
                taskDo.DeadlineDate,
                taskDo.DeadlineTime,
                taskDo.Duration,
                taskDo.StateId,
                taskDo.ActionId,
                taskDo.DependantTaskId,
                dependantTask != null ? dependantTask.State : null,
                taskDo.CreateAppointment);

            this.tasksRepository.Add(task);

            this.unitOfWork.Save();

            return new CreateResultDo() { Id = task.TaskId };
        }

        [Route("{id:int}")]
        [HttpGet]
        public TaskDo GetTaskDo(int id)
        {
            var task = this.tasksRepository.Find(id);
            if (task.UserId != this.userContext.UserId)
            {
                throw new Exception("You do not have permissions on this task");
            }

            return new TaskDo()
            {
                TaskId = task.TaskId,
                UserId = task.UserId,
                InternalImportance = task.InternalImportance,
                ExternalImportance = task.ExternalImportance,
                Clearness = task.Clearness,
                Closeness = task.Closeness,
                Simplicity = task.Simplicity,
                GroupId = task.GroupId,
                Title = task.Title,
                Description = task.Description,
                Tag = !string.IsNullOrEmpty(task.Tag) ? task.Tag.Replace(TaskManagerConstants.Splitter, ", ") : null,
                Thumbnail = task.Thumbnail,
                DeadlineDate = task.Deadline,
                DeadlineTime = task.DeadlineTime,
                Duration = task.Duration,
                PostponeDeadline = task.PostponeDeadline,
                StateId = task.StateId,
                ActionId = task.ActionId,
                DependantTaskId = task.DependantTaskId,
                CreateAppointment = task.CreateAppointment,
                LateStart = task.Deadline.HasValue ? new DateTime((TimeSpan.FromTicks(task.Deadline.Value.Ticks) - TimeSpan.FromHours(task.Duration ?? 0)).Ticks) : (DateTime?)null
            };
        }

        [Route("{id:int}")]
        [HttpPost]
        [Transaction]
        public void UpdateTask(int id, TaskDo taskDo)
        {
            var task = this.tasksRepository.Find(id);
            if (task.UserId != this.userContext.UserId)
            {
                throw new Exception("You do not have permissions on this task");
            }

            var dependantTask = taskDo.DependantTaskId.HasValue ? this.tasksRepository.Find(taskDo.DependantTaskId.Value) : null;
            task.Modify(
                taskDo.InternalImportance,
                taskDo.ExternalImportance,
                taskDo.Clearness,
                taskDo.Closeness,
                taskDo.Simplicity,
                taskDo.Title,
                taskDo.Description,
                taskDo.Tag,
                taskDo.Thumbnail,
                taskDo.DeadlineDate,
                taskDo.DeadlineTime,
                taskDo.Duration,
                taskDo.StateId,
                taskDo.ActionId,
                taskDo.DependantTaskId,
                dependantTask != null ? dependantTask.State : null,
                taskDo.CreateAppointment);

            this.unitOfWork.Save();
        }

        [Route("{id:int}/postpone")]
        [HttpPost]
        [Transaction]
        public void PostponeTask(int id, DateTime newDeadline)
        {
            var task = this.tasksRepository.Find(id);
            if (task.UserId != this.userContext.UserId)
            {
                throw new Exception("You do not have permissions on this task");
            }

            task.Postpone(newDeadline);

            this.unitOfWork.Save();
        }

        [Route("{id:int}")]
        [HttpDelete]
        [Transaction]
        public void RemoveTask(int id)
        {
            var task = this.tasksRepository.Find(id);
            if (task.UserId != this.userContext.UserId)
            {
                throw new Exception("You do not have permissions on this task");
            }
            
            this.tasksRepository.Remove(task);

            this.unitOfWork.Save();
        }

        [Route("overloading")]
        [HttpGet]
        public object CheckForOverloading()
        {
            var date = this.tasksRepository.GetFirstOverloadedDay(this.userContext.UserId);
            
            return new
            {
                hasOverloadedDay = date.HasValue,
                overloadedDay = date,
            };
        }

        [Route("statistics")]
        [HttpGet]
        public TaskStatisticsVo GetStatistics()
        {
            var tasks = this.dbContextAccessor.DbContext.Set<TaskModel>()
                .Where(u => u.UserId == this.userContext.UserId);

            var actionsDic = tasks
                .Where(t => t.ActionId.HasValue)
                .Select(s => s.ActionId.Value)
                .GroupBy(s => s)
                .AsEnumerable()
                .ToDictionary(s => s.Key, s => s.Count());

            ActionsCountVo actionsCount = new ActionsCountVo()
            {
                Transfered = actionsDic.ContainsKey(Domain.Aggregates.Tasks.Action.Transfer.ActionId) ? actionsDic[Domain.Aggregates.Tasks.Action.Transfer.ActionId] : 0,
                Trashed = actionsDic.ContainsKey(Domain.Aggregates.Tasks.Action.Trash.ActionId) ? actionsDic[Domain.Aggregates.Tasks.Action.Trash.ActionId] : 0,
                Clarified = actionsDic.ContainsKey(Domain.Aggregates.Tasks.Action.Clarify.ActionId) ? actionsDic[Domain.Aggregates.Tasks.Action.Clarify.ActionId] : 0,
                Defered = actionsDic.ContainsKey(Domain.Aggregates.Tasks.Action.Defer.ActionId) ? actionsDic[Domain.Aggregates.Tasks.Action.Defer.ActionId] : 0,
                Delegated = actionsDic.ContainsKey(Domain.Aggregates.Tasks.Action.Delegate.ActionId) ? actionsDic[Domain.Aggregates.Tasks.Action.Delegate.ActionId] : 0,
                Executed = actionsDic.ContainsKey(Domain.Aggregates.Tasks.Action.Execute.ActionId) ? actionsDic[Domain.Aggregates.Tasks.Action.Execute.ActionId] : 0,
                FollowedUp = actionsDic.ContainsKey(Domain.Aggregates.Tasks.Action.FollowUp.ActionId) ? actionsDic[Domain.Aggregates.Tasks.Action.FollowUp.ActionId] : 0,
                Scheduled = actionsDic.ContainsKey(Domain.Aggregates.Tasks.Action.Schedule.ActionId) ? actionsDic[Domain.Aggregates.Tasks.Action.Schedule.ActionId] : 0,
                Simplified = actionsDic.ContainsKey(Domain.Aggregates.Tasks.Action.Simplify.ActionId) ? actionsDic[Domain.Aggregates.Tasks.Action.Simplify.ActionId] : 0
            };

            var allWaitingTimes = tasks.Where(s => s.StartedOn.HasValue).AsEnumerable().Select(t => t.WaitingTime);
            var allTasksThisMonth = tasks.Where(t => t.Deadline.HasValue && t.Deadline.Value.Month == DateTime.Now.Month);
            var percentageCompletedTasksCurrentMonth = allTasksThisMonth.Count() > 0 ? (100 * allTasksThisMonth.Where(t => t.CompletedOn.HasValue && DbFunctions.DiffDays(t.CompletedOn.Value, t.Deadline.Value) >= 0).Count()) / allTasksThisMonth.Count() : 0;
            var allTasksToday = tasks.Where(t => t.Deadline.HasValue && DbFunctions.DiffDays(t.Deadline.Value, DateTime.Now) == 0);
            var percentageCompletedTasksToday = allTasksToday.Count() > 0 ? (100 * allTasksToday.Where(t => t.CompletedOn.HasValue && DbFunctions.DiffDays(t.CompletedOn.Value, t.Deadline.Value) >= 0).Count()) / allTasksToday.Count() : 0;
            var percentageIncompletedTasksToday = allTasksToday.Count() > 0 ? (100 * allTasksToday.Where(t => !t.CompletedOn.HasValue).Count()) / allTasksToday.Count() : 0;

            return new TaskStatisticsVo()
            {
                ActionsCount = actionsCount,
                AverageWaitingTime = new EvaluatedTimeVo(TimeSpan.FromMilliseconds(allWaitingTimes.Select(ts => ts.Value.TotalMilliseconds).Average())),
                CompletedBeforeDeadlineCount = tasks.Where(t => t.CompletedOn.HasValue && t.Deadline.HasValue && t.CompletedOn <= t.Deadline).Count(),
                PercentageCompletedTasksCurrentMonth = percentageCompletedTasksCurrentMonth,
                PercentageCompletedTasksToday = percentageCompletedTasksToday,
                PercentageIncompletedTasksForToday = percentageIncompletedTasksToday
            };
        }

    }
}
