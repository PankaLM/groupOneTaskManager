using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Data.ViewObjects
{
    public class EvaluatedTimeVo
    {

        public EvaluatedTimeVo(TimeSpan? time = null)
        {
            if (time.HasValue)
            {
                this.Days = time.Value.Days;
                this.Minutes = time.Value.Minutes;
                this.Hours = time.Value.Hours;
                this.Time = time;
            }
        }

        public int? Days { get; set; }

        public int? Hours { get; set; }

        public int? Minutes { get; set; }

        public TimeSpan? Time { get; set; }
    }
}
