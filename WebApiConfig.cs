using Chubb.Tracker.Framework.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;

namespace Chubb.Tracker.TrackerReportingService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //Enable Basic Authentication based on configuration settings
            string RequiresAuth = ConfigurationManager.AppSettings.Get("RequiresAuth");
            if (RequiresAuth.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                config.Filters.Add(new BasicAuthenticationAttribute());
            }
        }
    }
}
