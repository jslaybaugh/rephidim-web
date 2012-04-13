using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.IO;
using Postal;
using Web.Code;
using Web.Models;
using Common;

namespace Web.Areas.Ajax.Controllers
{
	[Authorize]
	[AjaxHandleError]
	public class FilesController : Controller
	{

		public JsonResult Folders(string path)
		{
			return Json(FileUtility.GetFolders(path), JsonRequestBehavior.AllowGet);
		}

		public JsonResult Contents(string path)
		{
			return Json(FileUtility.GetFiles(path), JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult Email(string email, string path)
		{
			string root = ConfigurationManager.AppSettings["FilesRoot"];
			root = root.Trim().EndsWith(@"\") ? root = root.Substring(0, root.Length - 2) : root;

			var fullpath = string.Format(@"{0}{1}", ConfigurationManager.AppSettings["FilesRoot"], path.Replace("/", "\\"));

			var cookie = Request.Cookies["rephidim"];
			if (cookie == null)
			{
				cookie = new HttpCookie("rephidim");
			}
			cookie["email"] = email;
			cookie.Expires = DateTime.Now.AddMonths(12);
			Response.Cookies.Add(cookie);

			dynamic msg = new Email("FileAttach");
			msg.To = email;
			msg.Path = path;
			msg.Attach(new System.Net.Mail.Attachment(fullpath));
			msg.Send();

			return Json(true);
		}

		[HttpPost]
		public JsonResult NewFolder(string name, string path)
		{
			if (User.IsInRole("Files"))
			{
				string root = ConfigurationManager.AppSettings["FilesRoot"];
				root = root.Trim().EndsWith(@"\") ? root = root.Substring(0, root.Length - 2) : root;

				var fullpath = string.Format(@"{0}{1}", ConfigurationManager.AppSettings["FilesRoot"], path.Replace("/", "\\"));

				Directory.CreateDirectory(fullpath.EndsWith("\\") ? fullpath + name : fullpath + "\\" + name);

				return Json(true);
			}
			else
				throw new Exception("Access Denied");
		}

	}
}
