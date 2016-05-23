using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Data.ViewObjects
{
    public class ActionsCountVo
    {
        public int Executed { get; set; }

        public int Simplified { get; set; }

        public int Clarified { get; set; }

        public int FollowedUp { get; set; }

        public int Delegated { get; set; }

        public int Defered { get; set; }

        public int Scheduled { get; set; }

        public int Transfered { get; set; }

        public int Trashed { get; set; }
    }
}
