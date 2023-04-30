using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using AbcSpa.Controllers;

namespace AbcSpa
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

	    protected void Session_End(object sender, EventArgs e)
	    {
			dynamic toJson = Session["curr_User"];
		    if (Session["curr_User"] == null) return;

		    Session["curr_User"] = null;
		    int userId = toJson.Id;
		    
		    new SecurityController().UserLogout(userId);
		}

		void Session_Start(object sender, EventArgs e)
		{
			// Code that runs when a new session is started
			Session.Timeout = 10;	// time given in minutes
		}
	}
}
