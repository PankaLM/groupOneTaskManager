using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TaskManager.Common.UserContextModels;
using TaskManager.Domain.Aggregates.Tasks;
using TaskManager.Domain.Data.Common;
using TaskManager.Domain.Data.ViewObjects;

namespace TaskManager.Web.Api.Tasks.Controllers
{
    [RoutePrefix("api/noms/tasks")]

    public class TaskNomsController : ApiController
    {
        private IDbContextAccessor accessor;
        private UserContext userContext;

        public TaskNomsController(IDbContextAccessor accessor, UserContext userContext)
        {
            this.accessor = accessor;
            this.userContext = userContext;
        }

        [Route("")]
        [HttpGet]
        public IEnumerable<TaskValueVo> GetTasks(string term = null, int? taskId = null)
        {
            var query = this.accessor.DbContext.Set<TaskModel>()
                .Where(t => t.UserId == this.userContext.UserId);

            if (taskId.HasValue)
            {
                query = query.Where(t => t.TaskId != taskId.Value);
            }

            return query
                .AsEnumerable()
                .Select(t => new TaskValueVo(t.TaskId, t.Title, t.State == State.Done));
        }

        [Route("{id:int}")]
        [HttpGet]
        public ValueVo GetState(int id)
        {
            var task = this.accessor.DbContext.Set<TaskModel>()
                .Where(t => t.TaskId == id)
                .Single();

            return new TaskValueVo(task.TaskId, task.Title, task.State == State.Done);
        }
    }
}
