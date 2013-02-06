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
    public class MessagesController : Controller
    {
		[HttpGet]
		public JsonResult List()
		{
			return Json(DataAccess.GetAllMessages(), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult Edit(int id)
		{
			if (User.IsInRole("Messages"))
				return Json(DataAccess.GetSingleMessage(id), JsonRequestBehavior.AllowGet);
			else
				throw new Exception("Access Denied");
		}

		[HttpPost]
		public JsonResult Delete(int id)
		{
			if (User.IsInRole("Messages"))
				return Json(DataAccess.DeleteMessage(id));
			else
				throw new Exception("Access Denied");
		}

		[HttpPost]
		public JsonResult Save(int id, string value, string style, bool isActive, bool onHomePage, bool onLoginPage)
		{
			if (User.IsInRole("Messages"))
			{
				return Json(DataAccess.UpsertMessage(id, value, style, isActive, onHomePage, onLoginPage));
			}
			else
				throw new Exception("Access Denied.");
		}

    }
}
