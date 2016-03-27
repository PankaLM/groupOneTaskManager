namespace TaskManager.Common.UserContextModels
{
    public class UserContext
    {
        private int userId;

        protected UserContext()
        {
        }

        public UserContext(int userId)
        {
            this.userId = userId;
        }
        public virtual int UserId
        {
            get
            {
                return this.userId;
            }
        }
    }
}
