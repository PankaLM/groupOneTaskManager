using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Data.ViewObjects
{
    public class TaskValueVo : ValueVo
    {
        public TaskValueVo()
        {
        }

        public TaskValueVo(int id, string name, bool isDone)
            : base(id, name)
        {
            this.IsDone = isDone;
        }

        public bool IsDone { get; set; }
    }
}
