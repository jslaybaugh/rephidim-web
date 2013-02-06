using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.Models;

namespace Web.Models
{
	public class GlossaryView
	{
		public IEnumerable<GlossarySummary> Terms { get; set; }
		public GlossaryItem ActiveTerm { get; set; }
		public KeyValuePair<string,string> Version { get; set; }
	}
}