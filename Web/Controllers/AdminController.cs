using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using System.Text.RegularExpressions;

namespace Web.Controllers
{
    public class AdminController : Controller
    {
		[HttpGet]
        public ActionResult Parse()
        {
			var m = new AdminParseView();
			m.Expression = "(?imx) ";
            return View("Parse", m);

        }

		[HttpPost]
		public ActionResult Parse(AdminParseView m)
		{
			var reg = new Regex(m.Expression);
			m.EndResult = reg.Replace(m.Source, m.Replace);
			return View("Parse", m);
		}

    }
}
