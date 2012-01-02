using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
	public class DirectoryInfoResult
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public int DirectoryCount { get; set; }
		public bool New { get; set; }
		public int FileCount { get; set; }
	}
}