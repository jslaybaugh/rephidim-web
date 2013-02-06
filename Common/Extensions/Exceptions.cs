using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Exceptions
{
	public static string UnRoll(this Exception ex)
	{
		string msg = ex.Message
			.Replace(". See the inner exception for details.", "")
			.Replace("\r", "")
			.Replace("\n", "");

		if (ex.InnerException != null)
		{
			msg += ": " + ex.InnerException.UnRoll();
		}

		return msg;
	}
}
