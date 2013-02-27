using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Common.Models
{
	public class PeopleItem
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Phone { get; set; }
		public string Email { get; set; }
		public string Notes { get; set; }
		public bool Nursery { get; set; }
		public bool Security { get; set;}
		public bool FirstAid { get; set; }
		public bool Kitchen { get; set; }
		public bool IsActive { get; set; }
	}
}