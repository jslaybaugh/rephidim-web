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

    }
}
