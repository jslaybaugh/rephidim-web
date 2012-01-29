using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.Models;

namespace Web.Models
{
	public class AdminEmailsView
	{
		public bool IsAdmin { get; set; }
		public IEnumerable<string> Emails { get; set; }
		public string Email { get; set; }
	}
}