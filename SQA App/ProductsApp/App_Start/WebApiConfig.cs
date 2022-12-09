using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SQApp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            // ignore circular reference globally
            //config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            // just adding another controller will add a relative path to the api route
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{queryName}",
                defaults: new { queryName = RouteParameter.Optional }
            );
        }
    }
}
