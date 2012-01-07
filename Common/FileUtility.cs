using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Common.Models;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Common
{
	public static class FileUtility
	{

		public static IEnumerable<DirectoryInfoResult> GetFolders(string path)
		{
			string root = GetRoot();
			path = string.Format(@"{0}{1}", ConfigurationManager.AppSettings["FilesRoot"], path.Replace("/", "\\"));

			var dir = new DirectoryInfo(path);

			var dirs = dir.EnumerateDirectories().Select(x => new DirectoryInfoResult
			{
				Name = x.Name,
				Path = x.FullName.Replace(root, "").Replace(@"\", "/"),
				DirectoryCount = x.EnumerateDirectories().Count(),
				DateModified = x.LastWriteTime,
				DateCreated = x.CreationTime,
				IsNew = x.LastWriteTime.Subtract(x.CreationTime).TotalMinutes < 31 && x.CreationTime.Subtract(DateTime.Now).Duration().TotalDays < Convert.ToInt32(ConfigurationManager.AppSettings["days"]),
				IsModified = x.LastWriteTime.Subtract(x.CreationTime).TotalMinutes > 30 && x.LastWriteTime.Subtract(DateTime.Now).Duration().TotalDays < Convert.ToInt32(ConfigurationManager.AppSettings["days"]),
				FileCount = x.EnumerateFiles().Count(y => !y.Extension.MatchesTrimmed(".ini") && !y.Extension.MatchesTrimmed(".db") && !y.Extension.MatchesTrimmed(".lnk"))
			}).OrderBy(x => x.Name);

			return dirs;
		}

		public static IEnumerable<FileInfoResult> GetFiles(string path)
		{
			string root = GetRoot();
			path = string.Format(@"{0}{1}", ConfigurationManager.AppSettings["FilesRoot"], path.Replace("/", "\\"));

			var dir = new DirectoryInfo(path);

			var files = dir.EnumerateFiles()
				.Where(y => !y.Extension.MatchesTrimmed(".ini") && !y.Extension.MatchesTrimmed(".db") && !y.Extension.MatchesTrimmed(".lnk"))
				.Select(x => new FileInfoResult
				{
					Name = x.Name.Substring(0, x.Name.LastIndexOf(".")),
					Path = x.FullName.Replace(root, "").Replace(@"\", "/"),
					Size = PrintFileSize(x.Length),
					DateModified = x.LastWriteTime,
					DateCreated = x.CreationTime,
					IsNew = x.LastWriteTime.Subtract(x.CreationTime).TotalMinutes < 31 && x.CreationTime.Subtract(DateTime.Now).Duration().TotalDays < Convert.ToInt32(ConfigurationManager.AppSettings["days"]),
					IsModified = x.LastWriteTime.Subtract(x.CreationTime).TotalMinutes > 30 && x.LastWriteTime.Subtract(DateTime.Now).Duration().TotalDays < Convert.ToInt32(ConfigurationManager.AppSettings["days"]),
					Extension = x.Extension.ToLower().Replace(".", "")
				}).OrderBy(x => x.Name);

			return files;
		}

		public static IEnumerable<FileInfoResult> Search(string[] queryparts)
		{
			string root = GetRoot();
			var matchingFiles = new List<FileInfo>();
			SearchFiles(root, queryparts, ref matchingFiles);
			return matchingFiles
				.Select(x => new FileInfoResult
				{
					Name = x.Name.Substring(0, x.Name.LastIndexOf(".")),
					Path = x.FullName.Replace(root, "").Replace(@"\", "/"),
					Size = FileUtility.PrintFileSize(x.Length),
					DateModified = x.LastWriteTime,
					DateCreated = x.CreationTime,
					IsNew = x.LastWriteTime.Subtract(x.CreationTime).TotalMinutes < 31 && x.CreationTime.Subtract(DateTime.Now).Duration().TotalDays < Convert.ToInt32(ConfigurationManager.AppSettings["days"]),
					IsModified = x.LastWriteTime.Subtract(x.CreationTime).TotalMinutes > 30 && x.LastWriteTime.Subtract(DateTime.Now).Duration().TotalDays < Convert.ToInt32(ConfigurationManager.AppSettings["days"]),
					Extension = x.Extension.ToLower().Replace(".", "")
				})
				.OrderBy(x => x.Name)
				.Distinct(new PropertyComparer<FileInfoResult>("Name"));
		}

		public static IEnumerable<FileInfoResult> GetRecentFiles()
		{
			string root = GetRoot();
			var matchingFiles = new List<FileInfo>();
			RecentFiles(root, ref matchingFiles);

			return matchingFiles
				.Select(x => new FileInfoResult
				{
					Name = x.Name.Substring(0, x.Name.LastIndexOf(".")),
					Path = x.FullName.Replace(root, "").Replace(@"\", "/"),
					Size = FileUtility.PrintFileSize(x.Length),
					DateModified = x.LastWriteTime,
					DateCreated = x.CreationTime,
					IsNew = x.LastWriteTime.Subtract(x.CreationTime).TotalMinutes < 31 && x.CreationTime.Subtract(DateTime.Now).Duration().TotalDays < Convert.ToInt32(ConfigurationManager.AppSettings["days"]),
					IsModified = x.LastWriteTime.Subtract(x.CreationTime).TotalMinutes > 30 && x.LastWriteTime.Subtract(DateTime.Now).Duration().TotalDays < Convert.ToInt32(ConfigurationManager.AppSettings["days"]),
					Extension = x.Extension.ToLower().Replace(".", "")
				})
				.OrderBy(x => x.Name)
				.Distinct(new PropertyComparer<FileInfoResult>("Name"));
		}



		private static void RecentFiles(string path, ref List<FileInfo> matchingFiles)
		{
			var dir = new DirectoryInfo(path);
			var files = dir.GetFiles();

			var found = files
				.ToList()
				.Where(x => x.LastWriteTime.Subtract(DateTime.Now).Duration().TotalDays < Convert.ToInt32(ConfigurationManager.AppSettings["days"]) || x.CreationTime.Subtract(DateTime.Now).Duration().TotalDays < Convert.ToInt32(ConfigurationManager.AppSettings["days"]));

			matchingFiles.AddRange(found);

			var dirs = dir.GetDirectories();

			if (dirs.Count() > 0)
			{
				foreach (var subdir in dirs)
				{
					RecentFiles(subdir.FullName, ref matchingFiles);
				}
			}
		}


		private static void SearchFiles(string path, string[] queryParts, ref List<FileInfo> matchingFiles)
		{
			string root = GetRoot();

			var dir = new DirectoryInfo(path);
			var files = dir.GetFiles();

			var regexp = string.Join("", queryParts.Select(x => "(?=.*" + x + ")"));
			var found = files
				.ToList()
				.Where(x => Regex.IsMatch(x.FullName.Replace(root, ""), regexp, RegexOptions.IgnoreCase) && !x.Extension.MatchesTrimmed(".ini") && !x.Extension.MatchesTrimmed(".db") && !x.Extension.MatchesTrimmed(".lnk"));

			matchingFiles.AddRange(found);

			var dirs = dir.GetDirectories();

			if (dirs.Count() > 0)
			{
				foreach (var subdir in dirs)
				{
					SearchFiles(subdir.FullName, queryParts, ref matchingFiles);
				}
			}
		}
		private static string GetRoot()
		{
			string root = ConfigurationManager.AppSettings["FilesRoot"];
			root = root.Trim().EndsWith(@"\") ? root = root.Substring(0, root.Length - 1) : root;

			return root;
		}

		private static string PrintFileSize(Int64 size)
		{
			if (size < 1024) return "< 1 KB";
			else if (size < 1024 * 1000) return string.Format("{0} KB", Math.Ceiling(size / 1000.0));
			else return string.Format("{0:N1} MB", size / (1000.0 * 1024));
		}
	}
}