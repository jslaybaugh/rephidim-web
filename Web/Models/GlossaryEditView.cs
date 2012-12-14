using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
	public class GlossaryEditView
	{
		[Required]
		public int Id { get; set; }

		[Required]
		public string Term { get; set; }

		[Required]
		public string Definition { get; set; }

		[Required]
		public DateTime DateModified { get; set; }
	}
}