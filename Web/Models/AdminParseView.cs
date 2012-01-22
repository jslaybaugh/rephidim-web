using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
	public class AdminParseView
	{
		public string Expression { get; set; }
		public string Replace { get; set; }
		public string Source { get; set; }
		public string EndResult { get; set; }
	}
}