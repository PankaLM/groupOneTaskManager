using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Common.Jobs
{
    public interface IJob
    {
        string Name { get; }

        TimeSpan Period { get; }

        void Action();
    }
}
