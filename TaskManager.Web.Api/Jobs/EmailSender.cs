using Autofac.Features.OwnedInstances;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
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
                return TimeSpan.FromMinutes(300);
            }
        }

        private readonly Regex EmailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

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
                    DateTime currentDate = DateTime.Now.Date;
                    var items = (from t in dbContextAccessor.Value.DbContext.Set<TaskModel>()
                                .Where(d => d.Deadline.HasValue && !d.Notified && DbFunctions.DiffDays(currentDate, d.Deadline) == 1)
                                    join u in dbContextAccessor.Value.DbContext.Set<User>()
                                            .Where(u => !string.IsNullOrEmpty(u.Email)) on t.UserId equals u.UserId
                                    select new
                                    {
                                        Task = t,
                                        Email = u.Email
                                    })
                                .ToList();

                    Regex emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

                    foreach (var item in items.Where(i => EmailRegex.Match(i.Email).Success))
                    {
                        MailMessage mailMessage = new MailMessage(defaultSender.Address, item.Email);
                        mailMessage.Subject = "Pending task";
                        mailMessage.Body = string.Format("Hi, you have pending task \"{0}\" with deadline \"{1}\".", item.Task.Title, item.Task.Deadline.Value.Date);
                        mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                        mailMessage.IsBodyHtml = true;

                        smtpClient.Send(mailMessage);
                        item.Task.MarkAsNotified();
                    }

                    dbContextAccessor.Value.DbContext.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in Email Sender: {0}", e);
                }
            }
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
