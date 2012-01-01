using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
	public class StaticController : Controller
	{

		public ActionResult Show(string path)
		{
			var m = new StaticView();
			m.Path = path;

			return View("Show", m);
		}

	}
}
