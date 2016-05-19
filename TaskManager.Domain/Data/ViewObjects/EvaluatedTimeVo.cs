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
                this.Minutes = time.Value.Minutes - time.Value.Days * 24 * 60;
                this.Hours = time.Value.Hours - time.Value.Days * 24;
                this.Time = time;
            }
        }

        public int? Days { get; set; }

        public int? Hours { get; set; }

        public int? Minutes { get; set; }

        public TimeSpan? Time { get; set; }
    }
}
