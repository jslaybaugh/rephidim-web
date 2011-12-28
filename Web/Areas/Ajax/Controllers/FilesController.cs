using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.IO;
using Postal;

namespace Web.Areas.Ajax.Controllers
{
	[Authorize]
	public class FilesController : Controller
	{

		public JsonResult Folders(string path)
		{
			string root = ConfigurationManager.AppSettings["FilesRoot"];
			root = root.Trim().EndsWith(@"\") ? root = root.Substring(0, root.Length - 1) : root;

			path = string.Format(@"{0}{1}", ConfigurationManager.AppSettings["FilesRoot"], path.Replace("/", "\\"));

			var dir = new DirectoryInfo(path);

			var dirs = dir.EnumerateDirectories().Select(x => new
			{
				Name = x.Name,
				Path = x.FullName.Replace(root, "").Replace(@"\", "/"),
				DirectoryCount = x.EnumerateDirectories().Count(),
				New = x.LastWriteTime.Subtract(DateTime.Now).Duration().TotalDays < 8,
				FileCount = x.EnumerateFiles().Count(y => !y.Extension.MatchesTrimmed(".ini") && !y.Extension.MatchesTrimmed(".db") && !y.Extension.MatchesTrimmed(".lnk"))
			}).OrderBy(x => x.Name);

			return Json(dirs, JsonRequestBehavior.AllowGet);
		}

		public JsonResult Contents(string path)
		{
			string root = ConfigurationManager.AppSettings["FilesRoot"];
			root = root.Trim().EndsWith(@"\") ? root = root.Substring(0, root.Length - 1) : root;

			path = string.Format(@"{0}{1}", ConfigurationManager.AppSettings["FilesRoot"], path.Replace("/", "\\"));

			var dir = new DirectoryInfo(path);

			var files = dir.EnumerateFiles()
				.Where(y => !y.Extension.MatchesTrimmed(".ini") && !y.Extension.MatchesTrimmed(".db") && !y.Extension.MatchesTrimmed(".lnk"))
				.Select(x => new
			{
				Name = x.Name.Substring(0, x.Name.LastIndexOf(".")),
				Path = x.FullName.Replace(root, "").Replace(@"\", "/"),
				Size = PrintFileSize(x.Length),
				FileDate = x.LastWriteTime,
				New = x.LastWriteTime.Subtract(DateTime.Now).Duration().TotalDays < 8,
				Extension = x.Extension.ToLower().Replace(".", "")
			}).OrderBy(x => x.Name);

			return Json(files, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult Email(string email, string path)
		{
			try
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
			catch (Exception)
			{
				return Json(false);
			}
		}

		private string PrintFileSize(Int64 size)
		{
			if (size < 1024) return "< 1 KB";
			else if (size < 1024 * 1000) return string.Format("{0} KB", Math.Ceiling(size / 1000.0));
			else return string.Format("{0:N1} MB", size / (1000.0 * 1024));

		}

	}
}
