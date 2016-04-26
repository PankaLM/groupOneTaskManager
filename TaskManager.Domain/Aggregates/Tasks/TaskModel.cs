using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using TaskManager.Common;
using TaskManager.Common.UserContextModels;
using TaskManager.Domain.Aggregates.Users;
using TaskManager.Domain.Data.Common;

namespace TaskManager.Domain.Aggregates.Tasks
{
    public class TaskModel : IAggregateRoot
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
            DateTime? deadline,
            int? duration,
            int stateId,
            int? actionId,
            int? dependantTaskId,
            State dependantTaskState)
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
            this.Deadline = deadline;
            this.Duration = duration;

            this.ActionId = actionId;
            
            this.ModifyState(stateId);
            this.ModifyTag(tag);
            this.ModifyFlyScore();
            this.ModifyDependantTask(dependantTaskId, dependantTaskState);
            this.ModifyDate = DateTime.Now;
            this.Notified = this.Deadline.HasValue ? (this.ModifyDate - this.Deadline).Value.Days > 0 : true ;

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
            DateTime? deadline,
            int? duration,
            int stateId,
            int? actionId,
            int? dependantTaskId,
            State dependantTaskState)
        {
            this.InternalImportance = internalImportance;
            this.ExternalImportance = еxternalImportance;
            this.Clearness = clearness;
            this.Closeness = closeness;
            this.Simplicity = simplicity;
            this.Title = title;
            this.Description = description;
            this.Thumbnail = thumbnail;
            this.Deadline = deadline;
            this.Duration = duration;
            this.ActionId = actionId;

            this.ModifyTag(tag);
            this.ModifyFlyScore();
            this.ModifyState(stateId);
            this.ModifyDependantTask(dependantTaskId, dependantTaskState);

            this.ModifyDate = DateTime.Now;
            this.Notified = this.Deadline.HasValue ? (this.ModifyDate - this.Deadline).Value.Days > 0 : true;
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

        public int? Duration { get; private set; }

        public DateTime? PostponeDeadline { get; private set; }

        public int StateId { get; private set; }

        public State State
        {
            get
            {
                return State.GetById(this.StateId);
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

        public DateTime StartedOn { get; private set; }

        public DateTime? CompletedOn { get; private set; }

        public DateTime? ModifyDate { get; private set; }

        public bool Notified { get; private set; }

        public RecurringTaskGroup RecurringTaskGroup { get; private set; }

        public User User { get; private set; }

        public void Postpone(DateTime newDeadline)
        {
            this.PostponeDeadline = newDeadline;
        }

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
            this.Property(t => t.PostponeDeadline).HasColumnName("PostponeDeadline");
            this.Property(t => t.StateId).HasColumnName("StateId");
            this.Property(t => t.ActionId).HasColumnName("ActionId");
            this.Property(t => t.DependantTaskId).HasColumnName("DependantTaskId");
            this.Property(t => t.StartedOn).HasColumnName("StartedOn");
            this.Property(t => t.CompletedOn).HasColumnName("CompletedOn");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Notified).HasColumnName("Notified");

            // Relationships
            this.HasOptional(t => t.RecurringTaskGroup)
                .WithMany()
                .HasForeignKey(d => d.GroupId);
            this.HasRequired(t => t.User)
                .WithMany(t => t.Tasks)
                .HasForeignKey(d => d.UserId);
        }
    }
}
