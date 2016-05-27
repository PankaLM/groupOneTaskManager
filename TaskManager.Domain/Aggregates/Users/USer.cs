using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Web.Helpers;
using TaskManager.Domain.Aggregates.Tasks;

namespace TaskManager.Domain.Aggregates.Users
{
    public class User
    {
        public User()
        {
            this.Tasks = new List<TaskModel>();
            this.UserAchievements = new List<UserAchievement>();
        }

        public User(
            string username,
            string fullname,
            string email,
            string password)
        {
            this.Fullname = fullname;
            this.Username = username;
            this.Email = email;
            this.SetPassword(password);
            this.IsActive = true;
            this.FlyCutoff = 15;
        }

        public void Modify(
           string fullname,
           string email)
        {
            this.Fullname = fullname;
            this.Email = email;
        }

        public void SetFlyCutoff(int value)
        {
            this.FlyCutoff = value;
        }

        public int UserId { get; private set; }

        public int FlyCutoff { get; private set; }

        public string Username { get; private set; }

        public string Fullname { get; private set; }

        public string Email { get; private set; }

        public bool IsActive { get; private set; }

        //server only
        [JsonIgnore]
        public string PasswordHash { get; private set; }
        [JsonIgnore]
        public string PasswordSalt { get; private set; }

        //client only
        public string Password { get; private set; }

        public IList<TaskModel> Tasks { get; private set; }

        public IList<UserAchievement> UserAchievements { get; private set; }

        private void SetPassword(string password)
        {
            if (password == null)
            {
                this.PasswordSalt = null;
                this.PasswordHash = null;
            }
            else
            {
                this.PasswordSalt = Crypto.GenerateSalt();
                this.PasswordHash = Crypto.HashPassword(password + this.PasswordSalt);
            }
        }

        public bool VerifyPassword(string password)
        {
            return Crypto.VerifyHashedPassword(this.PasswordHash, password + this.PasswordSalt);
        }

        public void ChangePassword(string oldPassword, string newPassword)
        {
            if (this.VerifyPassword(oldPassword))
            {
                this.SetPassword(newPassword);
            }
            else
            {
                throw new Exception("Wrong password provided");
            }
        }
    }

    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            // Primary Key
            this.HasKey(t => t.UserId);

            // Properties
            this.Property(t => t.UserId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.Email)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Users");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.Username).HasColumnName("Username");
            this.Property(t => t.PasswordHash).HasColumnName("PasswordHash");
            this.Property(t => t.PasswordSalt).HasColumnName("PasswordSalt");
            this.Property(t => t.Fullname).HasColumnName("Fullname");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.FlyCutoff).HasColumnName("FlyCutoff");
            this.Ignore(u => u.Password);
        }
    }
}
