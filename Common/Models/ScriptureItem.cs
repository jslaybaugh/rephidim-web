using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Models
{
	public class ScriptureItem
	{
		public int Id { get; set; }
		public BookItem Book { get; set; }
		public int Chapter { get; set; }
		public int Verse { get; set;}
		public string Text { get; set; }
		public string Notes { get; set; }
		public bool IsNew { get; set; }
		public bool IsModified { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime DateModified { get; set; }
	}
}
