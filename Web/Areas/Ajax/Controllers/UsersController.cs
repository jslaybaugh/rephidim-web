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
	public class UsersController : Controller
	{
		[HttpGet]
		public JsonResult List()
		{
			if (User.IsInRole("Dev"))
				return Json(DataAccess.GetUsers(), JsonRequestBehavior.AllowGet);
			else
				throw new Exception("Access Denied");
		}

		[HttpGet]
		public JsonResult Edit(int id)
		{
			if (User.IsInRole("Dev"))
				return Json(DataAccess.GetSingleUser(id), JsonRequestBehavior.AllowGet);
			else
				throw new Exception("Access Denied");
		}
		
		[HttpPost]
		public JsonResult Save(int id, string name, string rights, string password, bool isActive)
		{
			if (User.IsInRole("Dev"))
			{
				return Json(DataAccess.UpsertUser(id, name, rights, password, isActive), JsonRequestBehavior.AllowGet);
			}
			else
				throw new Exception("Access Denied.");
		}

	}
}
