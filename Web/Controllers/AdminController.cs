using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Common;
using MvcContrib;

namespace Web.Controllers
{
	[Authorize]
    public class AdminController : Controller
    {
		[HttpGet]
        public ActionResult Parse()
        {
			if (!User.IsInRole("Dev")) return Redirect(Url.Action<HomeController>(x => x.Home()) + "?warning=" + Server.UrlEncode("You don't have access to that page"));
            return View("Parse");
        }

		[HttpGet]
		public ActionResult Messages()
		{
			if (!User.IsInRole("Messages")) return Redirect(Url.Action<HomeController>(x => x.Home()) + "?warning=" + Server.UrlEncode("You don't have access to that page"));
			var m = new AdminMessagesView();
			m.Messages = DataAccess.GetAllMessages();

			return View("Messages", m);
		}

		[HttpGet]
		public ActionResult Emails()
		{
			var m = new AdminEmailsView();
			m.IsAdmin = User.IsInRole("Emails");
			m.Emails = DataAccess.GetEmails();

			return View("Emails", m);
		}

    }
}
