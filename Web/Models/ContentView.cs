using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.Models;

namespace Web.Models
{
	public class ContentView
	{
		public string Path { get; set; }
		public IEnumerable<FileInfoResult> Files { get; set; }
	}
}