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
    public class ScriptureController : Controller
    {
		[HttpGet]
		public JsonResult Books()
		{
			return Json(DataAccess.GetBooks(), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult Verses(int bookId, int chapterNum)
		{
			return Json(DataAccess.GetVerses(bookId, chapterNum), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult Edit(int id)
		{
			if (User.IsInRole("Scripture"))
				return Json(DataAccess.GetSingleVerse(id), JsonRequestBehavior.AllowGet);
			else
				throw new Exception("Access Denied");
		}

		[HttpPost]
		public JsonResult Save(int id, string text, string notes, string translationId, bool? updateDate)
		{
			if (User.IsInRole("Scripture"))
			{
				return Json(DataAccess.UpdateVerse(id, text, notes, translationId, updateDate), JsonRequestBehavior.AllowGet);
			}
			else
				throw new Exception("Access Denied.");
		}

    }
}
