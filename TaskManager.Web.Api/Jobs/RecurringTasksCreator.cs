using Autofac.Features.OwnedInstances;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Hosting;
using TaskManager.Common.Jobs;
using TaskManager.Domain.Aggregates.Tasks;
using TaskManager.Domain.Aggregates.Users;
using TaskManager.Domain.Data.Common;

namespace TaskManager.Web.Api.Jobs
{
    public class RecurringTasksCreator : IJob, IRegisteredObject, IDisposable
    {
        private Func<Owned<IDbContextAccessor>> dbContextAccessorFactory;

        private readonly object jobLock = new object();

        public bool IsShuttingDown { get; private set; }

        private Timer timer;

        public RecurringTasksCreator(Func<Owned<IDbContextAccessor>> dbContextAccessorFactory)
        {
            this.timer = new Timer(this.DoAction);
            this.dbContextAccessorFactory = dbContextAccessorFactory;
            HostingEnvironment.RegisterObject(this);
        }

        public void DoAction(object sender)
        {
            if (this.IsShuttingDown)
            {
                return;
            }
            
            if (Monitor.TryEnter(this.jobLock))
            {
                try
                {
                    this.Action();
                }
                finally
                {
                    Monitor.Exit(this.jobLock);
                }
            }
        }
        public void Stop(bool immediate)
        {
            this.IsShuttingDown = true;

            if (immediate)
            {
                // wait for the lock to be sure the task has finished
                lock (this.jobLock)
                {
                    HostingEnvironment.UnregisterObject(this);
                }
            }
        }

        public string Name
        {
            get
            {
                return "Recurring Tasks Creator Job";
            }
        }

        public TimeSpan Period
        {
            get
            {
                return TimeSpan.FromSeconds(300);
            }
        }

        public void Start()
        {
            this.timer.Change(TimeSpan.FromSeconds(0), this.Period);
        }

        private static object lockRoot = new object();
        
        public void Action()
        {
            using (var dbContextAccessor = dbContextAccessorFactory())
            {
                try
                {
                    lock (lockRoot)
                    {
                        dbContextAccessor.Value.DbContext.ChangeTracker.DetectChanges();
                        DateTime currentDate = DateTime.Now;
                        var users = dbContextAccessor.Value.DbContext.Set<TaskModel>()
                            .Where(d => d.IsRecurringGroup)
                            .GroupBy(u => u.UserId)
                            .ToList();
                        foreach (var user in users)
                        {
                            foreach (var group in user.Select(g => g))
                            {
                                var allTasksDeadline = dbContextAccessor.Value.DbContext.Set<TaskModel>()
                                    .Where(t => t.GroupId.HasValue && t.GroupId.Value == group.TaskId)
                                    .OrderByDescending(t => t.Deadline.Value)
                                    .Select(s => s.Deadline);
                                DateTime? lastGroupTaskDeadline = allTasksDeadline.Any() ? allTasksDeadline.Take(1).Single() : null;
                                DateTime date = lastGroupTaskDeadline != null ? lastGroupTaskDeadline.Value.Date.AddDays((double)group.RecurringGroupInterval.Value) : group.RecurringGroupStartDate.Value;
                                if (date >= currentDate && allTasksDeadline.Any())
                                {
                                    continue;
                                }
                                else
                                {
                                    DateTime dateAfter10tasks = date.AddDays(10 * (int)group.RecurringGroupInterval);
                                    while (date < dateAfter10tasks)
                                    {
                                        TaskModel newTask = new TaskModel(
                                            group.UserId,
                                            group.TaskId,
                                            group.InternalImportance,
                                            group.ExternalImportance,
                                            group.Clearness,
                                            group.Closeness,
                                            group.Simplicity,
                                            group.Title,
                                            group.Description,
                                            group.Tag,
                                            date,
                                            group.DeadlineTime,
                                            group.Duration,
                                            group.CreateAppointment);
                                        dbContextAccessor.Value.DbContext.Set<TaskModel>().Add(newTask);
                                        date = date.AddDays((int)group.RecurringGroupInterval);
                                    }
                                }
                            }
                        }
                        if (dbContextAccessor.Value.DbContext.ChangeTracker.HasChanges())
                        {
                            dbContextAccessor.Value.DbContext.SaveChanges();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in Recurring Tasks Creator: {0}", e);
                }
            }
        }
        public void Dispose()
        {
            this.timer.Dispose();
        }
    }
}
