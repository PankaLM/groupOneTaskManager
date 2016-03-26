using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Data.Common.Owin
{
    public static class RerouteExtensions
    {
        public static IAppBuilder UseReroute(this IAppBuilder app, string fromPath, string toPath)
        {
            return app.Use<RerouteMiddleware>(fromPath, toPath);
        }
    }
}
