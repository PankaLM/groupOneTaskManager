using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Aggregates.Tasks
{
    public class RecurringTaskGroup
    {
        public RecurringTaskGroup()
        {
        }

        public int GroupId { get; private set; }

        public long Interval { get; private set; }

        public DateTime StartTime { get; private set; }
    }

    public class RecurringTaskGroupMap : EntityTypeConfiguration<RecurringTaskGroup>
    {
        public RecurringTaskGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.GroupId);

            // Properties
            this.Property(t => t.GroupId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Table & Column Mappings
            this.ToTable("RecurringTaskGroups");
            this.Property(t => t.GroupId).HasColumnName("GroupId");
            this.Property(t => t.Interval).HasColumnName("Interval");
            this.Property(t => t.StartTime).HasColumnName("StartTime");
        }
    }
}
