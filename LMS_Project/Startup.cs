using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using System.Collections.Generic;
using Owin;
using Hangfire.SqlServer;
using System.Diagnostics;
using Hangfire;
using LMS_Project.Services;
using System.Configuration;

[assembly: OwinStartup(typeof(LMS_Project.Startup))]

namespace LMS_Project
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Enable CORS (cross origin resource sharing) for making request using browser from different domains
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            OAuthAuthorizationServerOptions options = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                //The Path For generating the Toekn
                TokenEndpointPath = new PathString("/token"),
                //Setting the Token Expired Time (24 hours)
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                //MyAuthorizationServerProvIder class will valIdate the user credentials
                //ProvIder = new MyAuthorizationServerProvIder()
            };
            //Token Generations
            app.UseOAuthAuthorizationServer(options);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            app.UseHangfireAspNet(GetHangfireServers);
            app.UseHangfireDashboard();
            PermissionService.AutoMapRouter();
            RecurringJob.AddOrUpdate(
                "Lặp lại 1 phút 1 lần",
                () => PushAuto.PushOneMinute(),
                "*/1 * * * *");
            RecurringJob.AddOrUpdate(
                "Lặp lại 1 ngày 1 lần",
                () => PushAuto.PushOneDay(),
                "* * */1 * *");

            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
        }
        private IEnumerable<IDisposable> GetHangfireServers()
        {
            Hangfire.GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(ConfigurationManager.ConnectionStrings["DbContext"].ConnectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                });

            yield return new BackgroundJobServer();
        }
    }
}
