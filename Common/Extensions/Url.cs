using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

public static class Url
{

	public static string AppendQueryItems(this System.Uri uri, IDictionary<string, string> items)
	{
		var dest = uri.OriginalString;

		var queryItems = string.Join("&", items.Select(x => x.Key + "=" + HttpUtility.UrlEncode(x.Value)).ToArray());

		return dest + (dest.Contains("?") ? "&" : "?") + queryItems;
	}
}
