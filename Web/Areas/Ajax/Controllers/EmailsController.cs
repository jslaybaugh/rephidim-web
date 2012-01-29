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
    public class EmailsController : Controller
    {
		[HttpGet]
		public JsonResult List()
		{
			return Json(DataAccess.GetEmails(), JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult Delete(string email)
		{
			if (User.IsInRole("Emails"))
				return Json(DataAccess.DeleteEmail(email), JsonRequestBehavior.AllowGet);
			else
				throw new Exception("Access Denied");
		}

		[HttpPost]
		public JsonResult Save(string email)
		{
			var cookie = Request.Cookies["rephidim"];
			if (cookie == null)
			{
				cookie = new HttpCookie("rephidim");
			}
			cookie["email"] = email;
			cookie.Expires = DateTime.Now.AddMonths(12);
			Response.Cookies.Add(cookie);

			return Json(DataAccess.InsertEmail(email), JsonRequestBehavior.AllowGet);
		}

    }
}
