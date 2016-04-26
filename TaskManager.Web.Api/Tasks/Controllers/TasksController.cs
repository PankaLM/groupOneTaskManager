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

namespace TaskManager.Web.Api.Controllers
{
  //  [Authorize]
    [RoutePrefix("api/tasks")]
    public class TasksController : ApiController
    {
        private UserContext userContext;
        private IUnitOfWork unitOfWork;
        private ITasksRepository tasksRepository;

        public TasksController(
            UserContext userContext,
            IUnitOfWork unitOfWork,
            ITasksRepository tasksRepository)
        {
            this.userContext = userContext;
            this.unitOfWork = unitOfWork;
            this.tasksRepository = tasksRepository;
        }

        [Route("")]
        [HttpGet]
        public IEnumerable<TaskVo> GetTasks()
        {
            return this.tasksRepository.GetTasks(this.userContext.UserId);
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
                taskDo.Deadline,
                taskDo.Duration,
                taskDo.StateId,
                taskDo.ActionId,
                taskDo.DependantTaskId,
                dependantTask != null ? dependantTask.State : null);

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
                Deadline = task.Deadline,
                Duration = task.Duration,
                PostponeDeadline = task.PostponeDeadline,
                StateId = task.StateId,
                ActionId = task.ActionId,
                DependantTaskId = task.DependantTaskId
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
                taskDo.Deadline,
                taskDo.Duration,
                taskDo.StateId,
                taskDo.ActionId,
                taskDo.DependantTaskId,
                dependantTask != null ? dependantTask.State : null);

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
    }
}
