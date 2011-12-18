using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Code;
using Web.Models;

namespace Web.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			var m = new HomeView();
			m.RecentTerms = DataAccess.GetRecentTerms(7);
			m.Messages = DataAccess.GetActiveHomeMesssages();

			return View("Index", m);
		}

		public ActionResult Search(string query)
		{
			return null;
		}

	}
}
