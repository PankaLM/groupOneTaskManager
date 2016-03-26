using System.Web.Http;
using TaskManager.Data.UserContextModels;
using TaskManager.Domain.Data.Common;
using TaskManager.Domain.Data.Repositories.Tasks;

namespace TaskManager.Web.Api.Controllers
{
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
        public string GetTasks()
        {
            return "hello";
        }
    }
}
