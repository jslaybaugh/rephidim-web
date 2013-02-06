using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Elmah;

namespace Web.Code
{
	public class AjaxHandleErrorAttribute : HandleErrorAttribute
	{
		public override void OnException(ExceptionContext filterContext)
		{
			base.OnException(filterContext);


			// elmah work

			var e = filterContext.Exception;
			if (!filterContext.ExceptionHandled   // if unhandled, will be logged anyhow
				|| RaiseErrorSignal(e)			  // prefer signaling, if possible
				|| IsFiltered(filterContext))     // filtered?
			{ }

			filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;  // stupid IIS7

			HttpRequestBase request = filterContext.RequestContext.HttpContext.Request;

			var result = new ContentResult { ContentType = "text/html", ContentEncoding = Encoding.UTF8, Content = filterContext.Exception.UnRoll()};
			filterContext.Result = result;

		}

		// see http://stackoverflow.com/questions/766610/how-to-get-elmah-to-work-with-asp-net-mvc-handleerror-attribute

		private static bool RaiseErrorSignal(Exception e)
		{
			var context = HttpContext.Current;
			if (context == null) return false;

			var signal = ErrorSignal.FromContext(context);
			if (signal == null) return false;

			signal.Raise(e, context);
			return true;
		}

		private static bool IsFiltered(ExceptionContext context)
		{
			var config = context.HttpContext.GetSection("elmah/errorFilter") as ErrorFilterConfiguration;

			if (config == null) return false;

			var testContext = new ErrorFilterModule.AssertionHelperContext(context.Exception, HttpContext.Current);

			return config.Assertion.Test(testContext);
		}
	}
}