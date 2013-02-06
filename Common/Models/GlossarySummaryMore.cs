using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Common.Models
{
	public class GlossarySummaryMore : GlossarySummary
	{
		public DateTime DateCreated { get; set; }
		public DateTime? DateModified { get; set; }
		public bool IsNew { get; set; }
		public bool IsModified { get; set; }
	}
}