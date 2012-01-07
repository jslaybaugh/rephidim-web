using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

	public static class Lists
	{
		public static List<T> RemoveAndReturn<T>(this List<T> list, T item)
		{
			list.Remove(item);
			return list;
		}
		public static List<T> AddAndReturn<T>(this List<T> list, T item)
		{
			list.Add(item);
			return list;
		}
	}
