using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Data.ViewObjects
{
    public class RecurringTaskGroupTaskVo
    {
        public int TaskId { get; set; }

        public string Title { get; set; }

        public int Interval { get; set; }

        public int? Duration { get; set; }
    }
}
