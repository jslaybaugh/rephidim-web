using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Common;
using MvcContrib;
using System.IO;
using Common.Models;

namespace Web.Controllers
{
	[Authorize]
    public class AdminController : Controller
    {
		[HttpGet]
        public ActionResult Parse()
        {
			if (!User.IsInRole("Dev")) return Redirect(Url.Action<HomeController>(x => x.Home()) + "?warning=" + Server.UrlEncode("You don't have access to that page"));
            return View("Parse");
        }

		[HttpGet]
		public ActionResult Messages()
		{
			if (!User.IsInRole("Messages")) return Redirect(Url.Action<HomeController>(x => x.Home()) + "?warning=" + Server.UrlEncode("You don't have access to that page"));
			var m = new AdminMessagesView();
			m.Messages = DataAccess.GetAllMessages();

			return View("Messages", m);
		}

		[HttpGet]
		public ActionResult Emails()
		{

			var cookie = Request.Cookies["rephidim"];
			string lastEmail = "";
			if (cookie != null)
			{
				lastEmail = cookie["email"];
			}

			var m = new AdminEmailsView();
			m.IsAdmin = User.IsInRole("Emails");
			m.Emails = DataAccess.GetEmails();
			m.Email = lastEmail;

			return View("Emails", m);
		}

		[HttpGet]
		public ActionResult Unsubscribe(string email, string code)
		{
			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code)) ViewBag.Message = "Invalid or Missing Parameters";

			var em = DataAccess.DeleteEmail(email, code);

			if (em == 1)
			{
				ViewBag.Message = "Successfully unsubscribed " + email + "!!";
			}
			else
			{
				ViewBag.Message = "Email not found. " +  email + " is already unsubscribed or that code is not valid.";
			}

			return View("Unsubscribe");
		}

		[HttpGet]
		public ActionResult Upload()
		{
			if (!User.IsInRole("Dev")) return Redirect(Url.Action<HomeController>(x => x.Home()) + "?warning=Access Denied.");
			ViewBag.Message = "";
			return View("Upload");
		}

		[HttpPost]
		public ActionResult Upload(HttpPostedFileBase file)
		{
			List<string[]> parsedData = new List<string[]>();
			List<ScriptureItem> results = new List<ScriptureItem>();
			List<int> good = new List<int>();
			List<int> bad = new List<int>();

			try
			{
				if (file == null) throw new Exception("File not supplied.");
				if (!User.IsInRole("Dev")) return Redirect(Url.Action<HomeController>(x => x.Home()) + "?warning=Access Denied.");

				using (StreamReader reader = new StreamReader(file.InputStream))
				{
					string line;
					string[] row;

					while ((line = reader.ReadLine()) != null)
					{
						if (!string.IsNullOrEmpty(line.Trim()))
						{
							row = line.Split('\t');
							parsedData.Add(row);
						}
					}
				}

				if (parsedData != null && parsedData.Count() > 0)
				{
					int index = 1;
					foreach (var row in parsedData)
					{
						int bookid = Convert.ToInt16(row[0].Trim());
						int chapter = Convert.ToInt32(row[1].Trim());
						int verse = Convert.ToInt32(row[2].Trim());
						string translation = row[3] == "" ? "RK" : row[3].ToUpperInvariant().Trim();
						string content = row[4].Trim();

						var res = DataAccess.UpdateVerse(content, "", translation, bookid, chapter, verse, true);
						if (res != null)
						{
							results.Add(res);
							good.Add(index);
						}
						else
						{
							bad.Add(index);
						}

						index += 1;
					}
				}

				ViewBag.Message = string.Format("{0} rows parsed and then {1} records uploaded and overwritten!<ul><li>Good Rows: {2}.</li><li>Bad Rows: {3}.</li></ul>", parsedData.Count(), results.Count(), string.Join(", ", good.ToArray()), string.Join(", ", bad.ToArray()));
				return View("Upload");
			}
			catch (Exception ex)
			{
				ViewBag.Message = string.Format("{0} rows parsed and then {1} records uploaded and overwritten!<ul><li>Good Rows: {2}.</li><li>Bad Rows: {3}.</li></ul>Error Details: {4}", parsedData.Count(), results.Count(), string.Join(", ", good.ToArray()), string.Join(", ", bad.ToArray()), ex.Message);
				return View("Upload");
			}

			

		}

    }
}
