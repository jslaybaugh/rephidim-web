using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
	public class FileInfoResult
	{
		public string Name { get; set;}
		public string Path { get; set; }
		public string Size { get; set; }
		public DateTime FileDate { get; set; }
		public bool New { get; set; }
		public string Extension { get; set; }
	}
}