using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Code;
using Web.Models;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace Web.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		private List<dynamic> _matchingFiles;
		
		public ActionResult Home()
		{
			string root = ConfigurationManager.AppSettings["FilesRoot"];
			root = root.Trim().EndsWith(@"\") ? root = root.Substring(0, root.Length - 1) : root;
			
			RecentFiles(root);

			var m = new HomeView();
			m.RecentTerms = DataAccess.GetRecentTerms();
			m.Messages = DataAccess.GetActiveHomeMesssages();
			m.RecentFiles = _matchingFiles
				.Select(x => new FileInfoResult
				{
					Name = x.Name.Substring(0, x.Name.LastIndexOf(".")),
					Path = x.FullName.Replace(root, "").Replace(@"\", "/"),
					Size = FileUtility.PrintFileSize(x.Length),
					DateModified = x.LastWriteTime,
					DateCreated = x.CreationTime,
					IsNew = x.CreationTime.Subtract(DateTime.Now).Duration().TotalDays < Convert.ToInt32(ConfigurationManager.AppSettings["days"]),
					IsModified = x.LastWriteTime.Subtract(DateTime.Now).Duration().TotalDays < Convert.ToInt32(ConfigurationManager.AppSettings["days"]),
					Extension = x.Extension.ToLower().Replace(".", "")
				})
				.OrderBy(x => x.Name)
				.Distinct(new PropertyComparer<FileInfoResult>("Name"));

			return View("Home", m);
		}

		public ActionResult Search(string query)
		{
			if (string.IsNullOrEmpty(query))
			{
				return View("Search", new SearchView { OriginalQuery = "" });
			}
			string root = ConfigurationManager.AppSettings["FilesRoot"];
			root = root.Trim().EndsWith(@"\") ? root = root.Substring(0, root.Length - 1) : root;

			var queryParts = GetSearchParts(query);
			SearchFiles(root, queryParts);

			var m = new SearchView();
			m.OriginalQuery = query;
			m.MatchingFiles = _matchingFiles
				.Select(x => new FileInfoResult
				{
					Name = x.Name.Substring(0, x.Name.LastIndexOf(".")),
					Path = x.FullName.Replace(root, "").Replace(@"\", "/"),
					Size = FileUtility.PrintFileSize(x.Length),
					DateModified = x.LastWriteTime,
					DateCreated = x.CreationTime,
					IsModified = x.LastWriteTime.Subtract(DateTime.Now).Duration().TotalDays < Convert.ToInt32(ConfigurationManager.AppSettings["days"]),
					IsNew =  x.CreationTime.Subtract(DateTime.Now).Duration().TotalDays < Convert.ToInt32(ConfigurationManager.AppSettings["days"]),
					Extension = x.Extension.ToLower().Replace(".", "")
				})
				.OrderBy(x => x.Name)
				.Distinct(new PropertyComparer<FileInfoResult>("Name"));
			m.QueryParts = queryParts;
			m.MatchingTerms = DataAccess.SearchTerms(queryParts);

			return View("Search", m);

		}

		private void RecentFiles(string path)
		{
			var root = ConfigurationManager.AppSettings["FilesRoot"];
			if (_matchingFiles == null) _matchingFiles = new List<dynamic>();

			var dir = new DirectoryInfo(path);
			var files = dir.GetFiles();

			var found = files
				.ToList()
				.Where(x => x.LastWriteTime.Subtract(DateTime.Now).Duration().TotalDays < Convert.ToInt32(ConfigurationManager.AppSettings["days"]) || x.CreationTime.Subtract(DateTime.Now).Duration().TotalDays < Convert.ToInt32(ConfigurationManager.AppSettings["days"]));

			_matchingFiles.AddRange(found);

			var dirs = dir.GetDirectories();

			if (dirs.Count() > 0)
			{
				foreach (var subdir in dirs)
				{
					RecentFiles(subdir.FullName);
				}
			}
		}


		private void SearchFiles(string path, string[] queryParts)
		{
			var root = ConfigurationManager.AppSettings["FilesRoot"];
			if (_matchingFiles == null) _matchingFiles = new List<dynamic>();

			var dir = new DirectoryInfo(path);
			var files = dir.GetFiles();

			var regexp = string.Join("", queryParts.Select(x => "(?=.*" + x + ")"));
			var found = files
				.ToList()
				.Where(x => Regex.IsMatch(x.FullName.Replace(root, ""), regexp, RegexOptions.IgnoreCase) && !x.Extension.MatchesTrimmed(".ini") && !x.Extension.MatchesTrimmed(".db") && !x.Extension.MatchesTrimmed(".lnk"));

			_matchingFiles.AddRange(found);

			var dirs = dir.GetDirectories();

			if (dirs.Count() > 0)
			{
				foreach (var subdir in dirs)
				{
					SearchFiles(subdir.FullName, queryParts);
				}
			}
		}

		private string[] GetSearchParts(string query)
		{
			//Clean it up
			query = query.Replace("--", "").Replace("'", "");
			var quotes = query.Split(new char[] { '"' }, StringSplitOptions.None);
			var res = new List<string>();

			for (int i = 0; i < quotes.Length; i++)
			{
				if (i % 2 == 1)
				{
					res.Add(quotes[i].Trim());
				}
				else
				{
					string commonWords = ConfigurationManager.AppSettings["CommonWords"];

					var words = quotes[i].Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

					foreach (var word in words)
					{
						if (!Regex.IsMatch(word, commonWords, RegexOptions.IgnoreCase)) res.Add(word);
					}
				}
			}

			return res.ToArray();
		}

	}
}
