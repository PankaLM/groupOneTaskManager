﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Web.Api.Tasks.DataObjects
{
    public class TaskDo
    {
        public int TaskId { get; set; }

        public int UserId { get; set; }

        public bool InternalImportance { get; set; }

        public bool ExternalImportance { get; set; }

        public bool Clearness { get; set; }

        public bool Closeness { get; set; }

        public bool Simplicity { get; set; }

        public int? GroupId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Tag { get; set; }

        public string Thumbnail { get; set; }

        public DateTime? DeadlineDate { get; set; }

        public string DeadlineTime { get; set; }

        public int? Duration { get; set; }

        public int? StateId { get; set; }

        public int? ActionId { get; set; }

        public int? DependantTaskId { get; set; }

        public bool CreateAppointment { get; set; }

        public string LateStart { get; set; }

        public bool IsRecurringGroup { get; set; }

        public int? RecurringGroupIntervalInDays { get; set; }

        public DateTime? RecurringGroupStartDate { get; set; }
    }
}
