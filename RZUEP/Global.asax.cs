using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using System.Net;

namespace RZUEP
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalVariables.Aktualizacja = false;
        }

        protected void Application_End()
        {
            try
            {
                WebClient http = new WebClient();
                string Result = http.DownloadString(HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority);
            }
            catch (Exception ex)
            {
                string Message = ex.Message;
            }
        }
    }
}
