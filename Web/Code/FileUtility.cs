using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Code
{
	public static class FileUtility
	{
		public static string PrintFileSize(Int64 size)
		{
			if (size < 1024) return "< 1 KB";
			else if (size < 1024 * 1000) return string.Format("{0} KB", Math.Ceiling(size / 1000.0));
			else return string.Format("{0:N1} MB", size / (1000.0 * 1024));

		}
	}
}