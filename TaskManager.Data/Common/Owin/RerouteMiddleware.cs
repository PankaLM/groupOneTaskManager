﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Data.Common.Owin
{
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;
    public class RerouteMiddleware
    {
        private readonly AppFunc next;
        private string fromPath;
        private string toPath;

        public RerouteMiddleware(AppFunc next, string fromPath, string toPath)
        {
            this.next = next;
            this.fromPath = fromPath;
            this.toPath = toPath;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            if ((string)environment["owin.RequestPath"] == fromPath)
            {
                environment["owin.RequestPath"] = toPath;
            }
            return next(environment);
        }
    }
}
