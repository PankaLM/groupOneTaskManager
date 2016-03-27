using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Data.Common.Linq;
using TaskManager.Domain.Data.Common;

namespace TaskManager.Data.Common
{
    internal class UnitOfWork : IUnitOfWork, IDbContextAccessor
    {
        public static readonly string ContextName = "DbContext";

        private static readonly Func<EntityReference, EntityKey> RelatedEndCachedValueAccessor = ExpressionHelper.GetFieldAccessor<EntityReference, EntityKey>("_cachedForeignKey");

        private static object syncRoot = new object();

        private static DbCompiledModel compiledModel = null;

        private bool disposed = false;

        private DbContext context = null;

        public UnitOfWork(IEnumerable<IDbConfiguration> configurations)
        {
            Initialize(configurations);
        }

        DbContext IDbContextAccessor.DbContext
        {
            get
            {
                return InternalDbContext;
            }
        }

        private DbContext InternalDbContext
        {
            get
            {
                if (this.context == null)
                {
                    this.context = CreateContext();
                }

                return this.context;
            }
        }

        public void Save()
        {
            try
            {
                this.InternalDbContext.ChangeTracker.DetectChanges();

                var addedOrModifiedEntityEntries =
                    ((IObjectContextAdapter)this.InternalDbContext).ObjectContext.ObjectStateManager
                    .GetObjectStateEntries(EntityState.Added | EntityState.Modified)
                    .Where(e => !e.IsRelationship)
                    .ToList();

                foreach (ObjectStateEntry entry in addedOrModifiedEntityEntries)
                {
                    // skip if a parent entity was deleted and a dependent entity becomes detached
                    if (entry.State == EntityState.Detached)
                    {
                        continue;
                    }

                    if (entry.RelationshipManager.GetAllRelatedEnds()
                        .Any(re =>
                            re is EntityReference &&
                            re.RelationshipSet.ElementType.RelationshipEndMembers
                                .Any(rem => rem.Name == re.TargetRoleName &&
                                    rem.DeleteBehavior == OperationAction.Cascade) &&
                            RelatedEndCachedValueAccessor((EntityReference)re).EntityContainerName.Contains("EntityHasNullForeignKey")))
                    {
                        ((IObjectContextAdapter)this.InternalDbContext).ObjectContext.DeleteObject(entry.Entity);
                    }
                }

                this.InternalDbContext.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Console.WriteLine(
                            "Class: {0}, Property: {1}, Error: {2}",
                            validationErrors.Entry.Entity.GetType().FullName,
                            validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }

                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public DbContextTransaction BeginTransaction()
        {
            return this.InternalDbContext.Database.BeginTransaction();
        }

        public DbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return this.InternalDbContext.Database.BeginTransaction(isolationLevel);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing && this.context != null)
                {
                    this.context.Dispose();
                }

                this.context = null;
                this.disposed = true;
            }
        }

        private static void Initialize(IEnumerable<IDbConfiguration> configurations)
        {
            if (compiledModel == null)
            {
                lock (syncRoot)
                {
                    if (compiledModel == null)
                    {
                        Database.SetInitializer<DbContext>(null);

                        DbModelBuilder modelBuilder = new DbModelBuilder();

                        modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
                        modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

                        foreach (IDbConfiguration configuration in configurations)
                        {
                            configuration.AddConfiguration(modelBuilder);
                        }

                        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[ContextName].ConnectionString))
                        {
                            compiledModel = modelBuilder.Build(connection).Compile();
                        }

                        var initializationContext = CreateContext();
                    }
                }
            }
        }

        private static DbContext CreateContext()
        {
            var context = new DbContext("Name=" + ContextName, compiledModel);
            context.Configuration.LazyLoadingEnabled = false;
            context.Configuration.ProxyCreationEnabled = false;
            context.Configuration.UseDatabaseNullSemantics = true;
#if DEBUG
            context.Database.Log = s => Debug.WriteLine(s);
#endif

            return context;
        }
    }
}
