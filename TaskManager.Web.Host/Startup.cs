

using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.StaticFiles;
using Owin;
using System;
using System.Net.Http.Headers;
using System.Web.Http;
using TaskManager.Common.Jobs;
using TaskManager.Data;
using TaskManager.Data.Common;
using TaskManager.Data.Common.Owin;
using TaskManager.Domain;
using TaskManager.Web.Api;

namespace TaskManager.Web.Host
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = CreateAutofacContainer();

            this.Configure(app, container);
        }

        public static IContainer CreateAutofacContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new DomainModule());
            builder.RegisterModule(new DataModule());
            builder.RegisterModule(new WebApiModule());
            return builder.Build();
        }

        private void Configure(IAppBuilder app, IContainer container)
        {
            app.UseAutofacMiddleware(container);
            app.UseSession();
            ConfigureAuth(app, container);
            ConfigureWebApi(app, container);
            ConfigureStaticFiles(app);
            StartJobs(container);
        }

        private void ConfigureAuth(IAppBuilder app, IContainer container)
        {
            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/api/token"),
                Provider = container.Resolve<IOAuthAuthorizationServerProvider>(),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                AllowInsecureHttp = true
            });

            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            {
                Provider = container.Resolve<IOAuthBearerAuthenticationProvider>(),
                //override the token deserialization to be able to capture the properties
                AccessTokenProvider = new AuthenticationTokenProvider()
                {
                    OnReceive = (c) =>
                    {
                        c.DeserializeTicket(c.Token);

                        //check if invalid bearer token received
                        if (c.Ticket != null)
                        {
                            c.OwinContext.Environment["oauth.Properties"] = c.Ticket.Properties;
                        }
                    }
                }
            });
        }

        public static void ConfigureWebApi(IAppBuilder app, IContainer container)
        {
            HttpConfiguration config = new HttpConfiguration();

            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterHttpRequestMessage(config);
            builder.Update(container);

            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            config.Formatters.JsonFormatter.SerializerSettings = JsonSettings.JsonSerializerSettings;
            // fix for ie9 not supporting json
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local;

            config.MapHttpAttributeRoutes();

            app.UseAutofacWebApi(config);
            app.UseWebApi(config);
        }

        public static void ConfigureStaticFiles(IAppBuilder app)
        {
            app.UseReroute("/", "/index.html");
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = new PathString(""),
                FileSystem = new PhysicalFileSystem("./App"),
                ContentTypeProvider = new ContentTypeProvider(),
                ServeUnknownFileTypes = false,
                OnPrepareResponse = c =>
                {
                    if (c.OwinContext.Request.Path.Value == "/index.html")
                    {
                        c.OwinContext.Response.Headers.Add("Cache-Control", new[] { "no-cache", "no-store", "must-revalidate" });
                        c.OwinContext.Response.Headers.Add("Pragma", new[] { "no-cache" });
                        c.OwinContext.Response.Headers.Add("Expires", new[] { "0" });
                    }
                }
            });
        }

        public static void StartJobs(IContainer container)
        {
            var jobs = container.Resolve<IJob[]>();

            foreach (var job in jobs)
            {
                job.Action();
            }
        }
    }
}