using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
	public class MessageItem
	{
		public int Id { get; set; }
		public string Value { get; set; }
		public bool OnLoginPage { get; set; }
		public bool OnHomePage { get; set; }
		public bool IsActive { get; set; }
		public string Style { get; set; }
	}
}