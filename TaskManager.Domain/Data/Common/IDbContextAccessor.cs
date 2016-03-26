using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Data.Common
{
    public interface IDbContextAccessor : IDisposable
    {
        DbContext DbContext { get; }
    }
}
