using Owin;
using SecuritySystemService.Helpers;
using System.Web.Http;

namespace SecuritySystemService
{
    internal class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{seconds}",
                defaults: new { seconds = RouteParameter.Optional }
            );

            var jsonFormatter = config.Formatters.JsonFormatter;
            jsonFormatter.SerializerSettings = JsonHelper.Settings;
            config.Formatters.Clear();
            config.Formatters.Add(jsonFormatter);

            appBuilder.UseWebApi(config);
        }
    }
}
