using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Code;
using Web.Models;
using System.Configuration;
using System.IO;

namespace Web.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		private List<FileInfo> matchingFiles;
		
		public ActionResult Index()
		{
			var m = new HomeView();
			m.RecentTerms = DataAccess.GetRecentTerms(7);
			m.Messages = DataAccess.GetActiveHomeMesssages();

			return View("Index", m);
		}

		public ActionResult Search(string query)
		{
			string root = ConfigurationManager.AppSettings["FilesRoot"];
			root = root.Trim().EndsWith(@"\") ? root = root.Substring(0, root.Length - 1) : root;

			SearchFiles(root, query);

			var m = new SearchView();
			m.MatchingFiles = matchingFiles;
			m.MatchingTerms = DataAccess.SearchTerms(query);

			return View("Search", m);

		}


		private void SearchFiles(string path, string query)
		{
			if (matchingFiles == null) matchingFiles = new List<FileInfo>();

			var dir = new DirectoryInfo(path);
			var files = dir.GetFiles();

			matchingFiles.AddRange(files.Where(x => x.Name.ToLower().Contains(query.ToLower())));
			var dirs = dir.GetDirectories();

			if (dirs.Count() > 0)
			{
				foreach (var subdir in dirs)
				{
					SearchFiles(subdir.FullName, query);
				}
			}
		}

	}
}
