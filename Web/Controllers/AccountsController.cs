using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcContrib;
using Web.Models;
using System.Data.SqlClient;
using Web.Code;
using System.Web.Security;
using Common;

namespace Web.Controllers
{
	public class AccountsController : Controller
	{
		[HttpGet]
		public ActionResult Index()
		{
			return this.RedirectToAction<AccountsController>(x => x.Login(string.Empty));
		}

		public ActionResult Error()
		{
			throw new Exception("Error happened.");

		}

		[HttpGet]
		public ActionResult Login(string returnUrl)
		{
			var cookie = Request.Cookies["rephidim_user"];
			string lastUser = "rephidim";
			if (cookie != null)
			{
				lastUser = cookie["rephidim_user"];
			}

			var m = new LoginView();
			m.UserName = lastUser;
			m.ReturnUrl = returnUrl;
			m.Messages = DataAccess.GetActiveLoginMesssages();
			
			return View("Login", m);
		}

		[HttpPost]
		public ActionResult Login(LoginView m)
		{
			ViewBag.LoginTried = true;
			m.Messages = DataAccess.GetActiveLoginMesssages();
			if (!ModelState.IsValid) return View(m);

			var user = DataAccess.Authenticate(m.UserName, m.Password);

			if (user != null)
			{

				var cookie = Request.Cookies["rephidim_user"];
				if (cookie == null)
				{
					cookie = new HttpCookie("rephidim_user");
				}
				cookie["rephidim_user"] = m.UserName;
				cookie.Expires = DateTime.Now.AddMonths(12);
				Response.Cookies.Add(cookie);

				var ticket = new FormsAuthenticationTicket(1, user.Name, DateTime.Now, DateTime.Now.AddDays(30), false, user.Rights);
				var encTicket = FormsAuthentication.Encrypt(ticket);

				var authCookie = FormsAuthentication.GetAuthCookie(FormsAuthentication.FormsCookieName, false);
				authCookie.Value = encTicket;
				Response.SetCookie(authCookie);

				if (!string.IsNullOrEmpty(m.ReturnUrl)) return Redirect(m.ReturnUrl);
				else return this.RedirectToAction<HomeController>(x => x.Home()); ;
			}
			else
			{
				ModelState.AddModelError("Error", "Authentication Failed.");
				return View(m);
			}

		}

		[HttpGet]
		public ActionResult Logout()
		{
			if (Request.IsAuthenticated)
			{
				var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
				if (authCookie != null)
				{
					authCookie.Expires = DateTime.Now.AddDays(-1);
					Response.SetCookie(authCookie);
				}
			}
			return this.RedirectToAction<HomeController>(x => x.Home());
		}

		public ActionResult Docs(string id)
		{
			var m = new DocsView();
			m.Doc = id.ToLower().Trim();
			return View("Docs", m);
		}


	}
}
