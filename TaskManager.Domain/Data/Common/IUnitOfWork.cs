using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Data.Common
{
    public interface IUnitOfWork : IDisposable
    {
        DbContextTransaction BeginTransaction();

        DbContextTransaction BeginTransaction(IsolationLevel isolationLevel);

        void Save();
    }
}
