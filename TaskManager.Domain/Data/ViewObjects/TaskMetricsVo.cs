using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Data.ViewObjects
{
    public class TaskMetricsVo
    {
        public int TaskId { get; set; }

        public string Title { get; set; }

        public int FlyScore { get; set; }

        public DateTime? Deadline { get; set; }

        public int? Duration { get; set; }

        public string Tags { get; set; }

        // StartedOn - CreateDate
        public EvaluatedTimeVo WaitingTime { get; set; }

        // CompletedOn - StartedOn 
        public EvaluatedTimeVo ExecutionTime { get; set; }

        // CompletedOn - CreateDate
        public EvaluatedTimeVo CompletionTime { get; set; }


    }
}
