using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Data.ViewObjects
{
    public class TaskStatisticsVo
    {
        public ActionsCountVo ActionsCount { get; set; }

        public int CompletedBeforeDeadlineCount { get; set; }

        public EvaluatedTimeVo AverageWaitingTime { get; set; }

        public int PercentageCompletedTasksToday { get; set; }

        public int PercentageCompletedTasksCurrentMonth { get; set; }

        public int PercentageIncompletedTasksForToday { get; set; }
    }
}
