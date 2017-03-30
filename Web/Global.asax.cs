using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Security.Principal;
using System.Configuration;

namespace Web
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			//routes.MapRoute(
			//    "Glossary", // Route name
			//    "Glossary/{action}/{id}", // URL with parameters
			//    new { controller = "Glossary", action = "Term", id = UrlParameter.Optional }, // Parameter defaults
			//    new string[] { "Web.Controllers" }
			//);


			routes.MapRoute(
				"Glossary Print", // Route name
				"Glossary/Print/{id}", // URL with parameters
				new { controller = "Glossary", action = "Print", id = UrlParameter.Optional }, // Parameter defaults
				new string[] { "Web.Controllers" }
			);

			routes.MapRoute(
				"Glossary Term by Id", // Route name
				"Glossary/Term/{id}", // URL with parameters
				new { controller = "Glossary", action = "Term", id = UrlParameter.Optional }, // Parameter defaults
				new string[] { "Web.Controllers" }
			);

			routes.MapRoute(
				"Glossary Term by Name", // Route name
				"Glossary/{term}", // URL with parameters
				new { controller = "Glossary", action = "Term", term = UrlParameter.Optional}, // Parameter defaults
				new string[] { "Web.Controllers" }
			);

			routes.MapRoute(
				"Scripture", // Route name
				"Scripture/{book}/{chapter}/{verse}", // URL with parameters
				new { controller = "Scripture", action = "Read", book = UrlParameter.Optional, chapter = UrlParameter.Optional, verse = UrlParameter.Optional }, // Parameter defaults
				new string[] { "Web.Controllers" }
			); 

			routes.MapRoute(
				"Files", // Route name
				"Files/{action}/{*path}", // URL with parameters
				new { controller = "Files", action = "Browse", id = UrlParameter.Optional }, // Parameter defaults
				new string[] { "Web.Controllers" }
			);

			routes.MapRoute(
				"Static", // Route name
				"Static/{*path}", // URL with parameters
				new { controller = "Static", action = "Show", id = UrlParameter.Optional }, // Parameter defaults
				new string[] { "Web.Controllers" }
			);

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Home", action = "Home", id = UrlParameter.Optional }, // Parameter defaults
				new string[] { "Web.Controllers" }
			);

		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}

		protected void Application_AuthenticateRequest()
		{
			if (HttpContext.Current.Request.IsAuthenticated)
			{
				if (!HttpContext.Current.Request.RawUrl.ToLower().Contains("accounts"))
				{
					var cookie = HttpContext.Current.Request.Cookies["rephidim_forceout"];
					string val = "";
					if (cookie != null)
					{
						val = cookie["rephidim_forceout"];
					}

					bool pass = string.Compare(val, ConfigurationManager.AppSettings["ForceDate"], true) >= 0;
					if (!pass)
					{
						// kick em out
						HttpContext.Current.Response.Redirect("~/Accounts/Logout");
					}
				}


				var roles = (HttpContext.Current.User.Identity as FormsIdentity).Ticket.UserData.Split('|');
				HttpContext.Current.User = new GenericPrincipal(HttpContext.Current.User.Identity, roles);
			}
		}
	}
}