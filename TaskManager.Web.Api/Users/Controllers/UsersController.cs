using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TaskManager.Domain.Aggregates.Users;
using TaskManager.Domain.Data.Common;
using TaskManager.Web.Api.Users.DataObjects;

namespace TaskManager.Web.Api.Users
{
    public class UsersController : ApiController
    {
        private IDbContextAccessor dbContextAccessor;
        private IUnitOfWork unitOfWork;

        public UsersController(
            IDbContextAccessor dbContextAccessor,
            IUnitOfWork unitOfWork)
        {
            this.dbContextAccessor = dbContextAccessor;
            this.unitOfWork = unitOfWork;
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
        public IHttpActionResult UpdateUser(int id, UserDo userDo)
        {
            User user = dbContextAccessor.DbContext.Set<User>()
                .SingleOrDefault(e => e.UserId == id);

            user.Modify(userDo.Fullname, userDo.Email, userDo.Password);

            unitOfWork.Save();

            return Ok();
        }
    }
}
