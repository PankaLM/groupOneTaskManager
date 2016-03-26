using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Data.Common;

namespace TaskManager.Data.Common
{
    internal abstract class AggregateRepository<TEntity> : IAggregateRepository<TEntity>
    where TEntity : class, IAggregateRoot
    {
        protected IDbContextAccessor dbContextAccessor;

        public AggregateRepository(IDbContextAccessor dbContextAccessor)
        {
            this.dbContextAccessor = dbContextAccessor;
        }

        public abstract TEntity Find(int id);

        public void Add(TEntity entity)
        {
            this.dbContextAccessor.DbContext.Set<TEntity>().Add(entity);
        }

        public void Remove(TEntity entity)
        {
            this.dbContextAccessor.DbContext.Set<TEntity>().Remove(entity);
        }
    }
}
