using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Common.Models;

namespace Web.Models
{
	public class SearchView
	{
		public IEnumerable<FileInfoResult> MatchingFiles { get; set; }
		public IEnumerable<GlossaryItem> MatchingTerms { get; set; }
		public IEnumerable<ScriptureItem> MatchingVerses { get; set; }
		public string[] QueryParts { get; set; }
		public string OriginalQuery { get; set; }
	}
}