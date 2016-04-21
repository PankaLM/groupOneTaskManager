using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TaskManager.Common.UserContextModels;
using TaskManager.Domain.Aggregates.Users;
using TaskManager.Domain.Data.Common;
using TaskManager.Web.Api.Users.DataObjects;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace TaskManager.Web.Api.Users
{
    public class UsersController : ApiController
    {
        private IDbContextAccessor dbContextAccessor;
        private IUnitOfWork unitOfWork;
        private UserContext userContext;

        public UsersController(
            IDbContextAccessor dbContextAccessor,
            IUnitOfWork unitOfWork,
            UserContext userContext)
        {
            this.dbContextAccessor = dbContextAccessor;
            this.unitOfWork = unitOfWork;
            this.userContext = userContext;
        }

        [HttpPost]
        [Route("api/users")]
        public IHttpActionResult CreateUser(UserDo user)
        {
            User newUser = new User(user.Username, user.Fullname, user.Email, user.Password);

            dbContextAccessor.DbContext.Set<User>().Add(newUser);

            unitOfWork.Save();

            return Ok();
        }

        [HttpPost]
        [Route("api/users/{id}")]
        public IHttpActionResult UpdateUser(int id, UserEditDo userDo)
        {
            User user = dbContextAccessor.DbContext.Set<User>()
                .SingleOrDefault(e => e.UserId == id);

            user.Modify(userDo.Fullname, userDo.Email);
            if (!string.IsNullOrEmpty(userDo.NewPassword))
            {
                user.ChangePassword(userDo.OldPassword, userDo.NewPassword);
            }

            unitOfWork.Save();

            return Ok();
        }


        [HttpGet]
        [Route("api/users/profile")]
        public IHttpActionResult GetUserProfile()
        {
            User user = dbContextAccessor.DbContext.Set<User>()
                .SingleOrDefault(e => e.UserId == this.userContext.UserId);

            UserEditDo returnValue = new UserEditDo()
            {
                UserId = user.UserId,
                Username = user.Username,
                Fullname = user.Fullname,
                Email = user.Email,
                IsActive = user.IsActive
            };

            return Ok(returnValue);
        }

        [HttpPost]
        [Route("api/user/isCorrectPassword")]
        public IHttpActionResult IsCorrectPassword([FromBody] FormDataCollection formData)
        {
            bool isCorrect = false;
            if (formData["password"] != null)
            {
                User user = dbContextAccessor.DbContext.Set<User>()
                .SingleOrDefault(e => e.UserId == userContext.UserId);

                if (user != null)
                {
                    isCorrect = user.VerifyPassword(formData["password"]);
                }
            }

            return Ok(new
            {
                isCorrect = isCorrect
            });
        }
    }
}
