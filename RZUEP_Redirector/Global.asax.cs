using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace RZUEP_Redirector
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_BeginRequest()
        {
            UriBuilder uri = new UriBuilder(Context.Request.Url);
            uri.Scheme = "http";
            Response.Redirect(uri.ToString().Replace("rzuep.apphb.com", "rzuep.pl").Replace(":" + uri.Port.ToString(), ""));
        }
    }
}