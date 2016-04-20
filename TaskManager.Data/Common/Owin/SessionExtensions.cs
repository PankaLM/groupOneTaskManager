using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Data.Common.Owin
{
    public static class SessionExtensions
    {
        public static IAppBuilder UseSession(this IAppBuilder app)
        {
            return app.Use(typeof(SessionMiddleware));
        }

        public static string GetSessionKey(this HttpRequestMessage request)
        {
            return request.GetOwinEnvironment()[SessionMiddleware.OwinEnvironmentKey] as string;
        }
    }
}
