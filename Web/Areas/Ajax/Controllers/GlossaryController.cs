using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Code;

namespace Web.Areas.Ajax.Controllers
{
	[Authorize]
    public class GlossaryController : Controller
	{
		public JsonResult Details(string term)
		{
			return Json(DataAccess.GetSingleTerm(term), JsonRequestBehavior.AllowGet);
		}

    }
}
