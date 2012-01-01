using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Web.Models
{
	public class SearchView
	{
		public IEnumerable<FileInfo> MatchingFiles { get; set; }
		public IEnumerable<GlossaryItem> MatchingTerms { get; set; }
		public string[] QueryParts { get; set; }
		public string OriginalQuery { get; set; }
	}
}