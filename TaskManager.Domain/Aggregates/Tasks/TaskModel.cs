using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TaskManager.Common;
using TaskManager.Common.UserContextModels;
using TaskManager.Domain.Aggregates.Users;

namespace TaskManager.Domain.Aggregates.Tasks
{
    public class TaskModel
    {
        private TaskModel()
        {
        }

        public TaskModel(
            UserContext user,
            bool internalImportance,
            bool еxternalImportance,
            bool clearness,
            bool closeness,
            bool simplicity,
            string title,
            string description,
            string tag,
            string thumbnail,
            DateTime? deadlineDate,
            string deadlineTime,
            int? duration,
            int? stateId,
            int? actionId,
            int? dependantTaskId,
            State dependantTaskState,
            bool createAppointment,
            bool isRecurringGroup,
            int? recurringGroupIntervalInDays,
            DateTime? recurringGroupStartDate)
        {
            this.UserId = user.UserId;
            this.InternalImportance = internalImportance;
            this.ExternalImportance = еxternalImportance;
            this.Clearness = clearness;
            this.Closeness = closeness;
            this.Simplicity = simplicity;
            this.Title = title;
            this.Description = description;
            this.Thumbnail = thumbnail;
            this.Duration = duration;
            this.ModifyTag(tag);
            this.ModifyFlyScore();

            this.IsRecurringGroup = isRecurringGroup;
            if (isRecurringGroup)
            {
                this.SetRecurringGroupData(recurringGroupIntervalInDays.Value, recurringGroupStartDate.Value, deadlineTime);
            }
            else
            {
                this.ModifyDeadline(deadlineDate: deadlineDate, deadlineTime: deadlineTime);
                this.CreateAppointment = createAppointment;
                this.ActionId = actionId;
                this.ModifyState(stateId.Value);
                this.ModifyDependantTask(dependantTaskId, dependantTaskState);
            }

            this.ModifyDate = DateTime.Now;
            this.CreateDate = DateTime.Now;
            this.Notified = this.Deadline.HasValue ? (this.ModifyDate - this.Deadline).Value.Days > 0 : true;
        }

        public TaskModel(
            int userId,
            int groupId,
            bool internalImportance,
            bool еxternalImportance,
            bool clearness,
            bool closeness,
            bool simplicity,
            string title,
            string description,
            string tag,
            DateTime? deadlineDate,
            string deadlineTime,
            int? duration,
            bool createAppointment)
        {
            this.UserId = userId;
            this.InternalImportance = internalImportance;
            this.ExternalImportance = еxternalImportance;
            this.Clearness = clearness;
            this.Closeness = closeness;
            this.Simplicity = simplicity;
            this.Title = title;
            this.Description = description;
            this.GroupId = groupId;
            this.Duration = duration;
            this.ModifyTag(tag);
            this.ModifyFlyScore();

            this.IsRecurringGroup = false;
            this.ModifyDeadline(deadlineDate: deadlineDate, deadlineTime: deadlineTime);
            this.CreateAppointment = createAppointment;
            this.StateId = State.Initialized.StateId;
            this.ModifyState(State.Initialized.StateId);

            this.ModifyDate = DateTime.Now;
            this.CreateDate = DateTime.Now;
            this.Notified = this.Deadline.HasValue ? (this.ModifyDate - this.Deadline).Value.Days > 0 : true;
        }

        private void ModifyDeadline(
            DateTime? deadlineDate,
            string deadlineTime)
        {
            this.Deadline = deadlineDate;

            if (!string.IsNullOrEmpty(deadlineTime) && this.Deadline.HasValue)
            {
                var timeParts = deadlineTime.Split(':');
                this.Deadline = this.Deadline.Value.Date
                    .AddHours(int.Parse(timeParts[0].Trim()))
                    .AddMinutes(int.Parse(timeParts[1].Trim()));
            }
        }

        public void Modify(
            bool internalImportance,
            bool еxternalImportance,
            bool clearness,
            bool closeness,
            bool simplicity,
            string title,
            string description,
            string tag,
            string thumbnail,
            DateTime? deadlineDate,
            string deadlineTime,
            int? duration,
            int? stateId,
            int? actionId,
            int? dependantTaskId,
            State dependantTaskState,
            bool createAppointment,
            bool isRecurringGroup,
            int? recurringGroupIntervalInDays,
            DateTime? recurringGroupStartDate)
        {
            this.InternalImportance = internalImportance;
            this.ExternalImportance = еxternalImportance;
            this.Clearness = clearness;
            this.Closeness = closeness;
            this.Simplicity = simplicity;
            this.Title = title;
            this.Description = description;
            this.Thumbnail = thumbnail;
            this.Duration = duration;
            this.ModifyTag(tag);
            this.ModifyFlyScore();

            this.IsRecurringGroup = isRecurringGroup;
            if (isRecurringGroup)
            {
                this.SetRecurringGroupData(recurringGroupIntervalInDays.Value, recurringGroupStartDate.Value, deadlineTime);
            }
            else
            {
                this.ModifyDeadline(deadlineDate: deadlineDate, deadlineTime: deadlineTime);
                this.CreateAppointment = createAppointment;
                this.ActionId = actionId;
                this.ModifyState(stateId.Value);
                this.ModifyDependantTask(dependantTaskId, dependantTaskState);
            }

            this.ModifyDate = DateTime.Now;
        }

        public int TaskId { get; private set; }

        public int UserId { get; private set; }

        public bool InternalImportance { get; private set; }

        public bool ExternalImportance { get; private set; }

        public bool Clearness { get; private set; }

        public bool Closeness { get; private set; }

        public bool Simplicity { get; private set; }

        public int FlyScore { get; private set; }

        public int? GroupId { get; private set; }

        public string Title { get; private set; }

        public string Description { get; private set; }

        public string Tag { get; private set; }

        public string Thumbnail { get; private set; }

        public DateTime? Deadline { get; private set; }

        public string DeadlineTime
        {
            get
            {
                return this.Deadline.HasValue ? string.Format("{0:D2}:{1:D2}", this.Deadline.Value.Hour, this.Deadline.Value.Minute) : "";
            }
        }

        public DateTime? DeadlineDate
        {
            get
            {
                return this.Deadline.HasValue ? this.Deadline.Value.Date : (DateTime?)null;
            }
        }

        public TimeSpan? ExecutionTime
        {
            get
            {
                return this.CompletedOn.HasValue && this.StartedOn.HasValue ? (this.CompletedOn.Value - this.StartedOn.Value) : (TimeSpan?)null;
            }
        }


        public TimeSpan? CompletionTime
        {
            get
            {
                return this.CompletedOn.HasValue ? (this.CompletedOn.Value - this.CreateDate) : (TimeSpan?)null;
            }
        }

        public TimeSpan? WaitingTime
        {
            get
            {
                return this.StartedOn.HasValue ? (this.StartedOn.Value - this.CreateDate) : (TimeSpan?)null;
            }
        }

        public DateTime? LateStart
        {
            get
            {
                return this.Deadline.HasValue && !this.IsRecurringGroup ? new DateTime((TimeSpan.FromTicks(this.Deadline.Value.Ticks) - TimeSpan.FromHours(this.Duration ?? 0)).Ticks) : (DateTime?)null;
            }
        }
        public int? Duration { get; private set; }

        public int? StateId { get; private set; }

        public State State
        {
            get
            {
                return this.StateId.HasValue? State.GetById(this.StateId.Value) : null;
            }
        }

        public int? ActionId { get; private set; }

        public Action Action
        {
            get
            {
                return ActionId.HasValue ? Action.GetById(this.ActionId.Value) : null;
            }
        }

        public int? DependantTaskId { get; private set; }

        public bool CreateAppointment { get; private set; }

        public bool AppointmentSent { get; private set; }

        public DateTime? StartedOn { get; private set; }

        public DateTime? CompletedOn { get; private set; }

        public DateTime? CreateDate { get; private set; }

        public DateTime? ModifyDate { get; private set; }

        public bool Notified { get; private set; }

        public bool IsRecurringGroup { get; private set; }

        public int? RecurringGroupInterval { get; private set; }

        public DateTime? RecurringGroupStartDate { get; private set; }


        private void SetRecurringGroupData(
            int intervalInDays,
            DateTime recurringGroupStartDate,
            string deadlineTime)
        {
            this.RecurringGroupInterval = intervalInDays;
            this.RecurringGroupStartDate = recurringGroupStartDate;

            if (!string.IsNullOrEmpty(deadlineTime))
            {
                var timeParts = deadlineTime.Split(':');
                this.Deadline = DateTime.MinValue.Date
                    .AddHours(int.Parse(timeParts[0].Trim()))
                    .AddMinutes(int.Parse(timeParts[1].Trim()));
            }
        }

        public User User { get; private set; }

        private void ModifyTag(string tag)
        {
            if (!string.IsNullOrEmpty(tag))
            {
                this.Tag = tag.Trim()
                    .Replace(",", TaskManagerConstants.Splitter)
                    .Replace(",", TaskManagerConstants.Splitter)
                    .Replace("/", TaskManagerConstants.Splitter);
            }
        }

        private void ModifyFlyScore()
        {
            int result = 0;
            if (this.Clearness)
            {
                result += FlyCharacteristicWeights.Clearness;
            }
            if (this.Closeness)
            {
                result += FlyCharacteristicWeights.Closeness;
            }
            if (this.Simplicity)
            {
                result += FlyCharacteristicWeights.Simplicity;
            }
            if (this.InternalImportance)
            {
                result += FlyCharacteristicWeights.InternalImportance;
            }
            if (this.ExternalImportance)
            {
                result += FlyCharacteristicWeights.ExternalImportance;
            }
            this.FlyScore = result;
        }

        private void ModifyState(int stateId)
        {
            this.StateId = stateId;
            if (this.State == State.InProgress)
            {
                this.StartedOn = DateTime.Now;
            }
            else if (this.State == State.Done)
            {
                this.CompletedOn = DateTime.Now;
            }
        }

        public void MarkAsNotified()
        {
            this.Notified = true;
        }

        public void MarkAsSentAppointment()
        {
            this.AppointmentSent = true;
        }

        public void ModifyDependantTask(int? dependantTaskId, State dependantTaskState)
        {
            this.DependantTaskId = dependantTaskId;
            if (dependantTaskId.HasValue && dependantTaskState != State.Done)
            {
                this.StateId = State.Initialized.StateId;
            }
        }
    }

    public class TaskMap : EntityTypeConfiguration<TaskModel>
    {
        public TaskMap()
        {
            // Primary Key
            this.HasKey(t => t.TaskId);

            // Properties
            this.Property(t => t.TaskId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Title)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("Tasks");
            this.Property(t => t.TaskId).HasColumnName("TaskId");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.InternalImportance).HasColumnName("InternalImportance");
            this.Property(t => t.ExternalImportance).HasColumnName("ЕxternalImportance");
            this.Property(t => t.Clearness).HasColumnName("Clearness");
            this.Property(t => t.Closeness).HasColumnName("Closeness");
            this.Property(t => t.Simplicity).HasColumnName("Simplicity");
            this.Property(t => t.FlyScore).HasColumnName("FlyScore");
            this.Property(t => t.GroupId).HasColumnName("GroupId");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Tag).HasColumnName("Tag");
            this.Property(t => t.Thumbnail).HasColumnName("Thumbnail");
            this.Property(t => t.Deadline).HasColumnName("Deadline");
            this.Property(t => t.Duration).HasColumnName("Duration");
            this.Property(t => t.StateId).HasColumnName("StateId");
            this.Property(t => t.ActionId).HasColumnName("ActionId");
            this.Property(t => t.DependantTaskId).HasColumnName("DependantTaskId");
            this.Property(t => t.Notified).HasColumnName("Notified");
            this.Property(t => t.AppointmentSent).HasColumnName("AppointmentSent");
            this.Property(t => t.CreateAppointment).HasColumnName("CreateAppointment");
            this.Property(t => t.IsRecurringGroup).HasColumnName("IsRecurringGroup");
            this.Property(t => t.RecurringGroupInterval).HasColumnName("RecurringGroupInterval");
            this.Property(t => t.RecurringGroupStartDate).HasColumnName("RecurringGroupStartDate");
            this.Property(t => t.StartedOn).HasColumnName("StartedOn");
            this.Property(t => t.CompletedOn).HasColumnName("CompletedOn");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");

            // Relationships
            this.HasRequired(t => t.User)
                .WithMany(t => t.Tasks)
                .HasForeignKey(d => d.UserId);
        }
    }
}
