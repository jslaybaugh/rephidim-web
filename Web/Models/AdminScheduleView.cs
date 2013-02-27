using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.Models;

namespace Web.Models
{
	public class AdminScheduleView
	{
		public string Mode { get; set; }
		public IEnumerable<PeopleItem> People { get; set; }
	}
}