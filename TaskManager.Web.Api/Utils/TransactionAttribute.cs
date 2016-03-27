using Autofac;
using Autofac.Integration.Owin;
using System.Data.Entity;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using TaskManager.Domain.Data.Common;

namespace TaskManager.Web.Api.Utils
{
    public class TransactionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var owinContext = actionContext.Request.GetOwinContext();
            var transaction = owinContext.GetAutofacLifetimeScope().Resolve<IUnitOfWork>().BeginTransaction();

            owinContext.Set<DbContextTransaction>("taskManager.Transaction", transaction);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var owinContext = actionExecutedContext.Request.GetOwinContext();
            var transaction = owinContext.Get<DbContextTransaction>("taskManager.Transaction");

            try
            {
                if (actionExecutedContext.Exception != null)
                {
                    transaction.Rollback();
                }
                else
                {
                    transaction.Commit();
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }
    }
}
