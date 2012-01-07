using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Common.Models
{
	public class UserItem
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Password { get; set; }
		public string Rights { get; set; }
		public bool IsActive { get; set; }
	}
}