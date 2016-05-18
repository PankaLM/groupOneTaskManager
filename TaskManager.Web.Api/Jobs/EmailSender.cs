using Autofac.Features.OwnedInstances;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    public class EmailSender : IJob, IRegisteredObject, IDisposable
    {
        private Func<Owned<IDbContextAccessor>> dbContextAccessorFactory;

        private readonly object jobLock = new object();

        public bool IsShuttingDown { get; private set; }

        private Timer timer;

        public EmailSender(Func<Owned<IDbContextAccessor>> dbContextAccessorFactory)
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
                return "Email Sender Job";
            }
        }

        public TimeSpan Period
        {
            get
            {
                return TimeSpan.FromSeconds(300);
            }
        }

        private readonly Regex EmailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

        public void Start()
        {
            this.timer.Change(TimeSpan.FromSeconds(0), this.Period);
        }

        private readonly MailAddress DefaultSender = new MailAddress("taskManagerfmi@gmail.com");

        private readonly SmtpClient SmtpClient = new SmtpClient()
        {
            Host = "smtp.gmail.com",
            Port = 587,
            Credentials = new NetworkCredential("taskManagerfmi@gmail.com", "!QAZ2wsx1", ""),
            EnableSsl = true
        };

        private static object lockRoot = new object();

        private string ConstructCalendarAppointment(DateTime start, string subject, string description)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("BEGIN:VCALENDAR\n");
            sb.Append("METHOD:REQUEST\n");
            sb.Append("PRODID:TaskManager\n");
            sb.Append("BEGIN:VEVENT\n");
            sb.AppendFormat("ORGANIZER:MAILTO:{0}\n", DefaultSender);
            sb.AppendFormat("DTSTAMP:{0}\n", DateTime.Now.ToString(@"yyyyMMdd\THHmmss\Z"));
            sb.AppendFormat("DTSTART:{0}\n", start.ToString(@"yyyyMMdd\T000000\Z"));
            sb.AppendFormat("DTEND: {0} \n", start.AddDays(1).ToString(@"yyyyMMdd\T000000\Z"));
            sb.Append("CATEGORIES:MEETING\n");
            sb.Append("CLASS:PUBLIC\n");
            sb.AppendFormat("SUMMARY:{0}\n", subject);
            sb.AppendFormat("DESCRIPTION:{0} \n", description);
            sb.Append("END:VEVENT\n");
            sb.Append("END:VCALENDAR\n");
            return sb.ToString();
        }

        public void Action()
        {
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
                        MailMessage mailMessage = new MailMessage(DefaultSender.Address, item.Email);
                        mailMessage.Subject = "Pending task";
                        mailMessage.Body = string.Format("Hi, you have pending task \"{0}\" with deadline \"{1}\".", item.Task.Title, item.Task.Deadline.Value.Date);
                        mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                        mailMessage.IsBodyHtml = true;

                        SmtpClient.Send(mailMessage);
                        item.Task.MarkAsNotified();
                    }
                    lock (lockRoot)
                    {
                        dbContextAccessor.Value.DbContext.ChangeTracker.DetectChanges();
                        var appointments = (from t in dbContextAccessor.Value.DbContext.Set<TaskModel>()
                                                .Where(d => !d.AppointmentSent && d.CreateAppointment && d.Deadline.HasValue)
                                         join u in dbContextAccessor.Value.DbContext.Set<User>()
                                                 .Where(u => !string.IsNullOrEmpty(u.Email)) on t.UserId equals u.UserId
                                         select new
                                         {
                                             Task = t,
                                             Email = u.Email
                                         })
                                        .ToList();

                        foreach (var item in appointments.Where(i => EmailRegex.Match(i.Email).Success))
                        {
                            MailMessage msg = new MailMessage(DefaultSender.Address, item.Email);
                            string appointmentData = ConstructCalendarAppointment(item.Task.Deadline.Value, item.Task.Title, item.Task.Description);
                            byte[] data = Encoding.UTF8.GetBytes(appointmentData);
                            AlternateView appointment = new AlternateView(new MemoryStream(data), "text/calendar");
                            appointment.ContentType.Parameters.Add("method", "REQUEST");
                            msg.AlternateViews.Add(appointment);
                            msg.Subject = item.Task.Title;
                            msg.Body = item.Task.Description;

                            SmtpClient.Send(msg);
                            item.Task.MarkAsSentAppointment();
                        }
                        dbContextAccessor.Value.DbContext.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in Email Sender: {0}", e);
                }
            }
        }
        public void Dispose()
        {
            this.timer.Dispose();
        }
    }
}
