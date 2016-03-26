using System;

namespace TaskManager.Data.UserContextModels
{
    public class UnathorizedUserContext : UserContext
    {
        public UnathorizedUserContext()
        {
        }

        public override int UserId
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
