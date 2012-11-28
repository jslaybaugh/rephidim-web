using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Web.Models;
using System.Configuration;
using Postal;
using MvcContrib;
using Common;

namespace Web.Controllers
{
	public class FilesController : Controller
	{
		[Authorize]
		public ActionResult Browse(string path)
		{
			var m = new FileView();
			m.Path = path;

			return View("Browse", m);
		}

		public ActionResult Options(string path)
		{
			var m = new Models.FileOptions();
			m.File = new FileInfo(GetData(path).Path).ToFileInfoResult();

			return View("_Options", m);
		}

		public ActionResult Folders(string path)
		{
			ViewBag.Folders = FileUtility.GetFolders(string.IsNullOrEmpty(path) ? "" : path);

			return View("_Folders");
		}

		// this remains open for good reader [Authorize]
		public ActionResult Open(string path)
		{
			var m = GetData(path);
			return File(m.Path, m.ContentType);
		}

		//[Authorize]
		public ActionResult Download(string path)
		{
			var m = GetData(path);
			return File(m.Path, m.ContentType, m.Name);
		}

		[Authorize]
		[HttpPost]
		public ActionResult Upload(HttpPostedFileBase file, string path)
		{
			try
			{
				var orig = path;
				if (file == null) throw new Exception("File not supplied.");
				if (!User.IsInRole("Files")) return Redirect(Url.Action<FilesController>(x => x.Browse(null)) + "?warning=Access Denied.");

				string root = ConfigurationManager.AppSettings["FilesRoot"];
				root = root.Trim().EndsWith(@"\") ? root = root.Substring(0, root.Length - 2) : root;

				if (!path.StartsWith("/")) path = "/" + path;

				path = string.Format(@"{0}{1}", ConfigurationManager.AppSettings["FilesRoot"], path.Replace("/", "\\"));

				var temp = path.EndsWith("\\") ? (path + file.FileName) : (path + "\\" + file.FileName);

				file.SaveAs(temp);
				return Redirect(Url.Action<FilesController>(x => x.Browse(orig)) + "?success=File Saved!");
			}
			catch (Exception ex)
			{
				return Redirect(Url.Action<FilesController>(x => x.Browse(null)) + "?error=" + Server.UrlEncode(ex.Message));
			}

		}

		private dynamic GetData(string path)
		{
			string root = ConfigurationManager.AppSettings["FilesRoot"];
			root = root.Trim().EndsWith(@"\") ? root = root.Substring(0, root.Length - 2) : root;

			if (!path.StartsWith("/")) path = "/" + path;

			path = string.Format(@"{0}{1}", ConfigurationManager.AppSettings["FilesRoot"], path.Replace("/", "\\"));

			var f = new FileInfo(path);
			string contentType = "application/octet-stream";
			switch (f.Extension.ToLower().Replace(".", ""))
			{
				case "txt": contentType = "text/plain"; break;
				case "htm":
				case "html": contentType = "text/html"; break;
				case "rtf": contentType = "text/richtext"; break;
				case "jpg":
				case "jpeg": contentType = "image/jpeg"; break;
				case "gif": contentType = "image/gif"; break;
				case "bmp": contentType = "image/bmp"; break;
				case "mpg":
				case "mpeg": contentType = "video/mpeg"; break;
				case "avi": contentType = "video/avi"; break;
				case "pdf": contentType = "application/pdf"; break;
				case "doc":
				case "dot": contentType = "application/msword"; break;
				case "csv":
				case "xls":
				case "xlt": contentType = "application/x-msexcel"; break;
				default: contentType = "application/octet-stream"; break;
			}

			return new { Path = path, ContentType = contentType, Name = f.Name };
		}


	}
}