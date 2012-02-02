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
using Common;
using Common.Models;

namespace Web.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		
		public ActionResult Home()
		{			
			var m = new HomeView();
			m.RecentVerses = DataAccess.GetRecentVerses();
			m.RecentTerms = DataAccess.GetRecentTerms();
			m.Messages = DataAccess.GetActiveHomeMessages();
			m.RecentFiles = FileUtility.Recent();

			return View("Home", m);
		}

		public ActionResult Search(string query)
		{
			if (string.IsNullOrEmpty(query))
			{
				return View("Search", new SearchView { OriginalQuery = "" });
			}

			var queryParts = GetSearchParts(query);

			var m = new SearchView();
			m.OriginalQuery = query;
			var start = DateTime.UtcNow;
			m.MatchingFiles = FileUtility.Search(queryParts);
			m.QueryParts = queryParts;
			m.MatchingVerses = new List<ScriptureItem>();

			if (!Regex.IsMatch(query, @"^[A-Za-z]{1}[ -]?\d{1,3}$"))
				m.MatchingVerses =  DataAccess.SearchVerses(queryParts);

			m.MatchingTerms = DataAccess.SearchTerms(queryParts);
			var end = DateTime.UtcNow;

			ViewBag.Duration = end.Subtract(start).TotalSeconds;

			return View("Search", m);

		}

		private string[] GetSearchParts(string query)
		{
			var books = DataAccess.GetBooks();
			var booklist = string.Join("|", books.Select(x=>x.Name + (x.Aliases != "" ? "|" + x.Aliases : "")).ToArray());
			query = Regex.Replace(query, "(\\b(?:" + booklist + ")[.,]?[ ]*(?:\\d{1,3})[:;]?(?:\\d{1,3})?)", @"""$1""");
				
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
