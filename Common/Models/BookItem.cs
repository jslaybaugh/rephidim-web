using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Models
{
	public class BookItem
	{
		public int Id { get; set;}
		public string Name { get; set; }
		public int Chapters { get; set; }
		public string Aliases { get; set; }
	}
}
