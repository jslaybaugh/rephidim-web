using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Common.Models;

namespace Web.Models
{
	public class LoginView
	{
		[Required(ErrorMessage = "User Name is required.")]
		public string UserName { get; set; }
		[Required(ErrorMessage = "Password is required.")]
		public string Password {get;set;}
		public bool Error { get; set; }
		public string ReturnUrl { get; set; }
		public IEnumerable<MessageItem> Messages { get; set; }
	}
}