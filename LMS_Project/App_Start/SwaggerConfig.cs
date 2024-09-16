using System.Web.Http;
using WebActivatorEx;
using LMS_Project;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace LMS_Project
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;
            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "Skill Hub");
                    c.IncludeXmlComments(string.Format(@"{0}\bin\LMS_Project.xml", System.AppDomain.CurrentDomain.BaseDirectory));
                    c.DescribeAllEnumsAsStrings();
                    c.ApiKey("apiKey")
                    .Description("API Key Authentication")
                    .Name("token")
                    .In("header");
                    c.RootUrl(req =>
                    {
                        var url = req.RequestUri.Scheme;

#if !DEBUG
                                                url += "s";
#endif
                        url += "://" + req.RequestUri.Authority + System.Web.VirtualPathUtility.ToAbsolute("~");
                        return url;
                    });
                })
                .EnableSwaggerUi(c =>
                {
                    c.EnableApiKeySupport("token", "header");
                });
        }
    }
}
