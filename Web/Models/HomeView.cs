using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
	public class HomeView
	{
		public IEnumerable<GlossaryItem> RecentTerms { get; set; }
		public IEnumerable<MessageItem> Messages { get; set; }
	}
}