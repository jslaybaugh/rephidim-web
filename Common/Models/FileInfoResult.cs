using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Common.Models
{
	public class FileInfoResult
	{
		public string Name { get; set;}
		public string Path { get; set; }
		public string Size { get; set; }
		public long Length { get; set; }
		public DateTime DateModified { get; set; }
		public DateTime DateCreated { get; set; }
		public bool IsNew { get; set; }
		public bool IsModified { get; set; }
		public string Extension { get; set; }
	}
}