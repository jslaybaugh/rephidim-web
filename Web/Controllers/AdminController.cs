using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using System.Text.RegularExpressions;
using System.Data;
using Common;

namespace Web.Controllers
{
    public class AdminController : Controller
    {
		[HttpGet]
        public ActionResult Parse()
        {
			var m = new AdminParseView();

			//var ds = new DataSet();
			//ds.ReadXml(@"C:\nasb.xml");

			//var books = ds.Tables[0];
			//var chapters = ds.Tables[1];
			//var verses = ds.Tables[2];

			////foreach (DataRow dr in books.Rows)
			////{
			////    var k = DataAccess.InsertBook(Convert.ToInt16(dr["book_Id"]) + 1, dr["name"].ToString());
			////}

			////foreach (DataRow dr in chapters.Rows)
			////{
			////    DataAccess.InsertChapter(Convert.ToInt16(dr["chapter_Id"]), Convert.ToInt16(dr["name"]), Convert.ToInt16(dr["book_Id"]) + 1);
			////}

			//int j = 1;
			//foreach (DataRow dr in verses.Rows)
			//{

			//    DataAccess.InsertVerse(j, Convert.ToInt16(dr["chapter_Id"]), dr["verse_Text"].ToString(), Convert.ToInt16(dr["name"]));
			//    j += 1;
			//}

            return View("Parse", m);

        }

    }
}
