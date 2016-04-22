using Autofac.Features.OwnedInstances;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Web.Hosting;
using TaskManager.Common.Jobs;
using TaskManager.Domain.Aggregates.Tasks;
using TaskManager.Domain.Aggregates.Users;
using TaskManager.Domain.Data.Common;

namespace TaskManager.Web.Api.Jobs
{
    public class EmailSender : IJob, IRegisteredObject, IDisposable
    {
        private Func<Owned<IDbContextAccessor>> dbContextAccessorFactory;

        private readonly object jobLock = new object();

        public bool IsShuttingDown { get; private set; }

        public EmailSender(Func<Owned<IDbContextAccessor>> dbContextAccessorFactory)
        {
            this.dbContextAccessorFactory = dbContextAccessorFactory;
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

            HostingEnvironment.RegisterObject(this);
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
                return "Email Sender Job";
            }
        }

        public TimeSpan Period
        {
            get
            {
                return TimeSpan.FromMinutes(1440);
            }
        }

        public void Action()
        {
            MailAddress defaultSender = new MailAddress("taskManagerfmi@gmail.com");
            SmtpClient smtpClient = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                Credentials = new NetworkCredential("taskManagerfmi@gmail.com", "!QAZ2wsx1", ""),
                EnableSsl = true
            };

            using (var dbContextAccessor = dbContextAccessorFactory())
            {
                try
                {
                    var items = (from t in dbContextAccessor.Value.DbContext.Set<TaskModel>()
                                .Where(d => DbFunctions.DiffDays(d.Deadline, DateTime.Now) <= 1 && !d.Notified)
                                    join u in dbContextAccessor.Value.DbContext.Set<User>() on t.UserId equals u.UserId
                                    select new
                                    {
                                        Task = t,
                                        Email = u.Email
                                    })
                                .ToList();

                    foreach (var item in items)
                    {
                        MailMessage mailMessage = new MailMessage();
                        mailMessage.Subject = "Pending task";
                        mailMessage.Body = string.Format("Hi, you have pending task \"{0}\" with deadline \"{1}\".", item.Task.Title, item.Task.Deadline);
                        mailMessage.From = defaultSender;
                        mailMessage.To.Add(new MailAddress(item.Email));
                        mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                        mailMessage.IsBodyHtml = true;
                        
                        smtpClient.Send(mailMessage);
                        item.Task.MarkAsNotified();
                    }

                    dbContextAccessor.Value.DbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
