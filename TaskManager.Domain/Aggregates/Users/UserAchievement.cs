using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Aggregates.Users
{
    public class UserAchievement
    {
        public int AchievementId { get; private set; }

        public int UserId { get; private set; }

        public User User { get; private set; }
    }

    public class UserAchievementMap : EntityTypeConfiguration<UserAchievement>
    {
        public UserAchievementMap()
        {
            // Primary Key
            this.HasKey(t => new { t.AchievementId, t.UserId });

            // Properties
            this.Property(t => t.AchievementId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.UserId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("UserAchievements");
            this.Property(t => t.AchievementId).HasColumnName("AchievementId");
            this.Property(t => t.UserId).HasColumnName("UserId");

            // Relationships
            this.HasRequired(t => t.User)
                .WithMany(t => t.UserAchievements)
                .HasForeignKey(d => d.UserId);

        }
    }
}
