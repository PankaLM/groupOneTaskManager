using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Data.Common
{
    public interface IAggregateRepository<TEntity>
     where TEntity : class, IAggregateRoot
    {
        TEntity Find(int id);

        void Add(TEntity entity);

        void Remove(TEntity entity);
    }
}
