using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

public static class Cookies
{
	public static string SafelyGetValue(this HttpCookieCollection cookies, string cookieName, string keyName = "")
	{
		try
		{
			if (cookies[cookieName] == null) return string.Empty;

			if (!string.IsNullOrEmpty(keyName)) // they want the subkey
			{
				return cookies[cookieName][keyName];
			}
			else
			{
				return cookies[cookieName].Value;
			}

		}
		catch (Exception)
		{
			return string.Empty;
			//throw;
		}
	}
}
