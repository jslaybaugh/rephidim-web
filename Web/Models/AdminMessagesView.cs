using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.Models;

namespace Web.Models
{
	public class AdminMessagesView
	{
		public IEnumerable<MessageItem> Messages { get; set; }
	}
}