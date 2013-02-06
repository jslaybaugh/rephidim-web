using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Common.Models
{
	public class GlossaryItem
	{
		public int Id { get; set; }
		public string Term { get; set; }
		public string Definition { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime? DateModified { get; set; }
		public bool IsNew { get; set; }
		public bool IsModified { get; set; }
	}
}