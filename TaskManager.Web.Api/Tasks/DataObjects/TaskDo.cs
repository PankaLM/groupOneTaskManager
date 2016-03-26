using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Web.Api.Tasks.DataObjects
{
    public class TaskDo
    {
        public int? TaskId { get; set; }

        public string Title { get; set; }
    }
}
