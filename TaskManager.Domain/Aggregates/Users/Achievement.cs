using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Aggregates.Users
{
    public class Achievement
    {
        public int AchievementId { get; private set; }

        public string Name { get; private set; }

        public string Alias { get; private set; }
    }

    public class AchievementMap : EntityTypeConfiguration<Achievement>
    {
        public AchievementMap()
        {
            // Primary Key
            this.HasKey(t => t.AchievementId);
            
            this.Property(t => t.AchievementId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Properties
            this.Property(t => t.Name)
                .IsRequired();

            this.Property(t => t.Alias)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("Achievements");
            this.Property(t => t.AchievementId).HasColumnName("AchievementId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Alias).HasColumnName("Alias");
        }
    }
}
