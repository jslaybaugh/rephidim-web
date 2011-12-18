using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Web.Models;
using System.Configuration;
using Postal;

namespace Web.Controllers
{
	[Authorize]
    public class FilesController : Controller
	{

		[Authorize]
		public ActionResult Index()
		{
			var m = new FileView();
			m.Root = ConfigurationManager.AppSettings["FilesRoot"];

			return View(m);
		}

		private dynamic GetData(string path)
		{
			string root = ConfigurationManager.AppSettings["FilesRoot"];
			root = root.Trim().EndsWith(@"\") ? root = root.Substring(0, root.Length - 2) : root;

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

		[Authorize]
		public ActionResult File(string path)
		{
			var m = GetData(path);
			return File(m.Path, m.ContentType);
		}
		[Authorize]
		public ActionResult Download(string path)
		{
			var m = GetData(path);
			return File(m.Path, m.ContentType, m.Name);
		}

		public ActionResult Email(string path)
		{
			string root = ConfigurationManager.AppSettings["FilesRoot"];
			root = root.Trim().EndsWith(@"\") ? root = root.Substring(0, root.Length - 2) : root;

			path = string.Format(@"{0}{1}", ConfigurationManager.AppSettings["FilesRoot"], path.Replace("/", "\\"));


			dynamic email = new Email("Example");
			email.To = "jslaybaugh@brushfiresoftware.com";
			email.Attach(new System.Net.Mail.Attachment(path));
			email.Send();

			return View();
		}

		// JSON Actions

		public JsonResult AjaxFolders(string path)
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
				FileCount = x.EnumerateFiles().Count()
			}).OrderBy(x => x.Name);

			return Json(dirs, JsonRequestBehavior.AllowGet);
		}

		public JsonResult AjaxContents(string path)
		{
			string root = ConfigurationManager.AppSettings["FilesRoot"];
			root = root.Trim().EndsWith(@"\") ? root = root.Substring(0, root.Length - 1) : root;

			path = string.Format(@"{0}{1}", ConfigurationManager.AppSettings["FilesRoot"], path.Replace("/", "\\"));

			var dir = new DirectoryInfo(path);

			var files = dir.EnumerateFiles().Select(x => new
			{
				Name = x.Name.Substring(0, x.Name.LastIndexOf(".")),
				Path = x.FullName.Replace(root, "").Replace(@"\", "/"),
				Size = PrintFileSize(x.Length),
				FileDate = x.LastWriteTime,
				Extension = x.Extension.ToLower().Replace(".", "")
			}).Where(x => x.Extension != "ini" && x.Extension != "db" && x.Extension != "lnk").OrderBy(x => x.Name);

			return Json(files, JsonRequestBehavior.AllowGet);
		}

		private string PrintFileSize(Int64 size)
		{
			if (size < 1024) return "< 1 kb";
			else if (size < 1024 * 1000) return string.Format("{0} kb", Math.Ceiling(size / 1000.0));
			else return string.Format("{0:N1} mb", size / (1000.0 * 1024));

		}

		//int dirCounter = 0;
		//int fileCounter = 0;
		//int queryCount = 0;
		//string query = "";
		//List<string> extensions = new List<string>();

		//public ActionResult Index()
		//{

		//    var startTime = DateTime.Now;

		//    WalkDirectories(@"F:\Jorin\");

		//    var endTime = DateTime.Now;

		//    ViewBag.Summary = string.Format("Done. {0} seconds, {1} directories, {2} files, {3} matches for '{4}'.", (endTime - startTime).TotalSeconds, dirCounter, fileCounter, queryCount, query);
		//    ViewBag.Extensions = "Extensions: " + string.Join(", ", extensions.Distinct().ToArray());
			
		//    return View();
		//}

		//private void WalkDirectories(string path)
		//{
		//    var dir = new DirectoryInfo(path);
		//    Console.WriteLine(dir.FullName);
		//    var files = dir.GetFiles();
		//    extensions.AddRange(files.Select(x => x.Extension).Distinct().ToList<string>());

		//    var matches = files.Where(x => x.Name.ToLower().Contains(query.ToLower())).Count();
		//    var dirs = dir.GetDirectories();
		//    fileCounter += files.Count();
		//    dirCounter += dirs.Count();
		//    queryCount += matches;
		//    foreach (var item in dirs)
		//    {
		//        WalkDirectories(item.FullName);
		//    }
		//}

    }
}
