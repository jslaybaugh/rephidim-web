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
		public JsonResult Save(MessageSaveView m)
		{
			if (User.IsInRole("Messages"))
			{
				return Json(DataAccess.UpsertMessage(m.Id, m.Value, m.Style, m.IsActive, m.OnHomePage, m.OnLoginPage));
			}
			else
				throw new Exception("Access Denied.");
		}

		public class MessageSaveView
		{
			public int Id { get; set; }
			[AllowHtml]
			public string Value { get; set; }
			public string Style { get; set; }
			public bool IsActive { get; set; }
			public bool OnHomePage { get; set; }
			public bool OnLoginPage { get; set; }
		}

    }
}
