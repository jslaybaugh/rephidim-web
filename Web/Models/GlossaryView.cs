using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
	public class GlossaryView
	{
		public IEnumerable<GlossarySummary> Terms { get; set; }
		public GlossaryItem ActiveTerm { get; set; }
		public VersionItem Version { get; set; }
	}
}