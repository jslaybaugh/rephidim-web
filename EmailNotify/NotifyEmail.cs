using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Postal;

namespace EmailNotify
{
	public class NotifyEmail : Email
	{
		public NotifyEmail(string name) : base(name) { }

		public IEnumerable<Common.Models.ScriptureItem> Verses { get;set; }
		public IEnumerable<Common.Models.GlossaryItem> Terms { get; set; }
		public IEnumerable<Common.Models.FileInfoResult> Files { get; set; }
		public string To { get; set; }
		public string AbsoluteRoot { get; set; }
	}
}
