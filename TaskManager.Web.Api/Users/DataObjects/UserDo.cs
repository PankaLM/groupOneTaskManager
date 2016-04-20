using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Web.Api.Users.DataObjects
{
    public class UserDo
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Fullname { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }

        public string Password { get; set; }
    }
}
