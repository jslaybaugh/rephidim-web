using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Common;

namespace Web.Controllers
{
	[Authorize]
    public class ScriptureController : Controller
	{
		public ActionResult Index(string book, int? chapter, int? verse)
		{
			var m = new ScriptureView();
			m.Book = book;
			m.Chapter = chapter;
			m.Verse = verse;

			return View("Index", m);
		}


    }
}
