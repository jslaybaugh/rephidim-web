using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Web.Code;

namespace Web.Controllers
{
	[Authorize]
    public class GlossaryController : Controller
    {
        public ActionResult Term(int? id)
        {
			var m = new GlossaryView();

			m.Terms = DataAccess.GetAllTerms();
			if (id.HasValue)
			{
				m.ActiveTerm = DataAccess.GetSingleTerm(id.Value);
			}
			m.Version = DataAccess.GetSingleVersion(1);

            return View("Term",m);
        }

		public JsonResult AjaxSingle(string term)
		{
			return Json(DataAccess.GetSingleTerm(term), JsonRequestBehavior.AllowGet);
		}

    }
}
