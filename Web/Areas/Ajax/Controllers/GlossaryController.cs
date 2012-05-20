using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Code;
using Common;

namespace Web.Areas.Ajax.Controllers
{
	[Authorize]
	[AjaxHandleError]
    public class GlossaryController : Controller
	{
		[HttpGet]
		public JsonResult List()
		{
			return Json(DataAccess.GetAllTerms(), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult Details(string term)
		{
			return Json(DataAccess.GetSingleTerm(term), JsonRequestBehavior.AllowGet);
		}
		
		[HttpGet]
		public JsonResult Edit(int id)
		{
			if (User.IsInRole("Glossary"))
				return Json(DataAccess.GetSingleTerm(id), JsonRequestBehavior.AllowGet);
			else
				throw new Exception("Access Denied");
		}

		[HttpPost]
		public JsonResult Save(int id, string term, string definition, bool? updateDate)
		{
			if (User.IsInRole("Glossary"))
			{
				return Json(DataAccess.UpsertTerm(id, term, definition, updateDate));
			}
			else
				throw new Exception("Access Denied.");
		}

		[HttpPost]
		public JsonResult Delete(int id)
		{
			if (User.IsInRole("Glossary"))
			{
				return Json(DataAccess.DeleteTerm(id));
			}
			else
				throw new Exception("Access Denied.");
		}

		[HttpPost]
		public JsonResult UpdateVersion(string version)
		{
			if (User.IsInRole("Glossary"))
			{
				return Json(DataAccess.UpdateKeyValue("GlossaryVersion",version));
			}
			else
				throw new Exception("Access Denied.");
		}

    }
}
