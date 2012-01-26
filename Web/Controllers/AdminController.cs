using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class AdminController : Controller
    {
		[HttpGet]
        public ActionResult Parse()
        {
			var m = new AdminParseView();
            return View("Parse", m);

        }

    }
}
