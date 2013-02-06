using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Code;
using Common;
using Postal;
using System.Configuration;

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
			{
				DataAccess.DeleteEmail(email);
				return Json("Email removed!");
			}
			else
			{
				var em = DataAccess.GetEmails().FirstOrDefault(x => x.Email.MatchesTrimmed(email));

				if (em == null) throw new Exception("Email address not found!");

				dynamic msg = new Email("Unsubscribe");
				msg.To = email;
				msg.AbsoluteRoot = ConfigurationManager.AppSettings["AbsoluteRoot"];
				msg.VerifyCode = em.VerifyCode;
				msg.Send();

				return Json("Verification email sent!");
			}
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

			return Json(DataAccess.InsertEmail(email));
		}

		[HttpPost]
		public JsonResult SubmitIssue(string email, string content, string url)
		{
			var cookie = Request.Cookies["rephidim"];
			if (cookie == null)
			{
				cookie = new HttpCookie("rephidim");
			}
			cookie["email"] = email;
			cookie.Expires = DateTime.Now.AddMonths(12);
			Response.Cookies.Add(cookie);

			dynamic msg = new Email("Issue");
			msg.To = "bobcausey@sbcglobal.net, jslaybaugh@gmail.com";
			msg.From = email;
			msg.Content = content;
			msg.Url = url;
			msg.Send();

			return Json(true);
		}

    }
}
