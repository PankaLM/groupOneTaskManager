using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Data.ViewObjects;

namespace TaskManager.Domain.Data.Repositories.Tasks
{
    public interface ITasksRepository
    {
        IEnumerable<TaskVo> GetTasks(int userId);
    }
}
