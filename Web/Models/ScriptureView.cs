﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.Models;

namespace Web.Models
{
	public class ScriptureView
	{
		public IEnumerable<BookItem> Books { get; set; }
		public BookItem Book { get; set; }
		public int? Chapter { get; set; }
		public int? Verse { get; set; }
	}
}