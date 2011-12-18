using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
	public class GlossaryItem
	{
		public int Id { get; set; }
		public string Term { get; set; }
		public string Definition { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime? DateModified { get; set; }
	}
}