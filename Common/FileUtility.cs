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
		private static string Root
		{
			get
			{
				string root = ConfigurationManager.AppSettings["FilesRoot"];
				root = root.Trim().EndsWith(@"\") ? root : root + @"\";

				return root;
			}
		}

		public static FileInfoResult ToFileInfoResult(this FileInfo file, DateTime? lastDate = null)
		{
			return new FileInfoResult
			{
				Name = file.Name.Substring(0, file.Name.LastIndexOf(".") < 0 ? 0 : file.Name.LastIndexOf(".")),
				Path = file.FullName.Replace(Root, "").Replace(@"\", "/"),
				Size = PrintFileSize(file.Length),
				Length = file.Length,
				DateModified = file.LastWriteTime,
				DateCreated = file.CreationTime,
				IsNew = file.LastWriteTime.Subtract(file.CreationTime).TotalMinutes < 31 && file.CreationTime > lastDate.Value,
				IsModified = file.LastWriteTime.Subtract(file.CreationTime).TotalMinutes > 30 && file.LastWriteTime > lastDate.Value,
				Extension = file.Extension.ToLower().Replace(".", "")
			};
		}

		public static DirectoryInfoResult ToDirectoryInfoResult(this DirectoryInfo dir, DateTime? lastDate = null)
		{
			return new DirectoryInfoResult
			{
				Name = dir.Name,
				Path = dir.FullName.Replace(Root, "").Replace(@"\", "/"),
				DirectoryCount = dir.EnumerateDirectories().Count(),
				DateModified = dir.LastWriteTime,
				DateCreated = dir.CreationTime,
				IsNew = dir.LastWriteTime.Subtract(dir.CreationTime).TotalMinutes < 31 && dir.CreationTime > lastDate.Value,
				IsModified = dir.LastWriteTime.Subtract(dir.CreationTime).TotalMinutes > 30 && dir.LastWriteTime > lastDate.Value,
				FileCount = dir.EnumerateFiles().Count(y => !y.Attributes.HasFlag(FileAttributes.System | FileAttributes.Hidden) && !y.Extension.MatchesTrimmed(".ini") && !y.Extension.MatchesTrimmed(".db") && !y.Extension.MatchesTrimmed(".lnk"))
			};
		}

		private static string MakeFullLocal(string path)
		{
			if (path.StartsWith("/")) path = path.Substring(1);

			return string.Format(@"{0}{1}", Root, path.Replace("/", "\\"));
		}

		public static IEnumerable<DirectoryInfoResult> GetFolders(string path, DateTime? lastDate = null)
		{
			if (lastDate == null) lastDate = DateTime.Now.AddDays(-Convert.ToInt32(ConfigurationManager.AppSettings["days"]));

			string localPath = MakeFullLocal(path);

			var dir = new DirectoryInfo(localPath);

			var dirs = dir.EnumerateDirectories().Select(x => x.ToDirectoryInfoResult(lastDate)).OrderBy(x => x.Name);

			return dirs;
		}

		public static IEnumerable<FileInfoResult> GetFiles(string path, DateTime? lastDate = null)
		{
			if (lastDate == null) lastDate = DateTime.Now.AddDays(-Convert.ToInt32(ConfigurationManager.AppSettings["days"]));
			string localPath = MakeFullLocal(path);

			var dir = new DirectoryInfo(localPath);

			var files = dir.EnumerateFiles()
				.Where(y => !y.Attributes.HasFlag(FileAttributes.System | FileAttributes.Hidden))
				.Where(y => !y.Extension.MatchesTrimmed(".ini") && !y.Extension.MatchesTrimmed(".db") && !y.Extension.MatchesTrimmed(".lnk"))
				.Select(x => x.ToFileInfoResult(lastDate))
				.OrderBy(x => x.Name);

			return files;
		}

		public static IEnumerable<FileInfoResult> Search(string[] queryparts, DateTime? lastDate = null)
		{
			if (lastDate == null) lastDate = DateTime.Now.AddDays(-Convert.ToInt32(ConfigurationManager.AppSettings["days"]));
			//var matchingFiles = SearchFiles(new Regex(string.Join("", queryparts.Select(x => "(?=.*" + x + ")")), RegexOptions.IgnoreCase));
			var matchingFiles = SearchFiles(queryparts);
			return matchingFiles
				.Where(y => !y.Attributes.HasFlag(FileAttributes.System | FileAttributes.Hidden))
				.Where(y => !y.Extension.MatchesTrimmed(".ini") && !y.Extension.MatchesTrimmed(".db") && !y.Extension.MatchesTrimmed(".lnk"))
				.Select(x => x.ToFileInfoResult(lastDate))
				.OrderBy(x => x.Name)
				.Distinct(new PropertyComparer<FileInfoResult>("Name"));
		}

		public static IEnumerable<FileInfoResult> Recent(DateTime? lastDate = null)
		{
			if (lastDate == null) lastDate = DateTime.Now.AddDays(-Convert.ToInt32(ConfigurationManager.AppSettings["days"]));
			var matchingFiles = RecentFiles(lastDate);
			return matchingFiles
				.Where(y => !y.Attributes.HasFlag(FileAttributes.System | FileAttributes.Hidden))
				.Where(y => !y.Extension.MatchesTrimmed(".ini") && !y.Extension.MatchesTrimmed(".db") && !y.Extension.MatchesTrimmed(".lnk"))
				.Select(x => x.ToFileInfoResult(lastDate))
				.OrderByDescending(x => x.IsNew ? x.DateCreated : x.DateModified).ThenBy(x => x.Name)
				.Distinct(new PropertyComparer<FileInfoResult>("Name"));
		}



		private static IEnumerable<FileInfo> RecentFiles(DateTime? lastDate = null)
		{
			if (lastDate == null) lastDate = DateTime.Now.AddDays(-Convert.ToInt32(ConfigurationManager.AppSettings["days"]));
			var dir = new DirectoryInfo(Root);
			return dir.EnumerateFiles("*", SearchOption.AllDirectories)
				.Where(x => x.LastWriteTime > lastDate.Value || x.CreationTime > lastDate.Value)
				.ToList();
		}

		//private static IEnumerable<FileInfo> SearchFiles(Regex reg)
		//{
		//    var dir = new DirectoryInfo(Root);
		//    return dir.EnumerateFiles("*", SearchOption.AllDirectories)
		//        .Where(x => reg.IsMatch(x.FullName)).ToList();
		//}

		private static IEnumerable<FileInfo> SearchFiles(string[] queryParts)
		{
			var dir = new DirectoryInfo(Root);

			//var results = dir.EnumerateFiles("*" + queryParts[0] + "*", SearchOption.AllDirectories);
			//for (int i = 1; i < queryParts.Length; i++)
			//{
			//    var nextResult = dir.EnumerateFiles("*" + queryParts[i] + "*", SearchOption.AllDirectories);
			//    results = results.Intersect(nextResult, new PropertyComparer<FileInfo>("FullName"));
			//}

			var results = dir.EnumerateFiles("*", SearchOption.AllDirectories).Where(x => x.FullName.ToUpperInvariant().Contains(queryParts[0].ToUpperInvariant()));
			if (queryParts.Length > 1)
			{
				for (int i = 1; i < queryParts.Length; i++)
				{
					var nextResult = dir.EnumerateFiles("*", SearchOption.AllDirectories).Where(x => x.FullName.ToUpperInvariant().Contains(queryParts[i].ToUpperInvariant())).ToList();
					results = results.Intersect(nextResult, new PropertyComparer<FileInfo>("FullName"));
				}
			}

			return results.ToList();
		}

		private static string PrintFileSize(Int64 size)
		{
			if (size < 1024) return "< 1 KB";
			else if (size < 1024 * 1000) return string.Format("{0} KB", Math.Ceiling(size / 1000.0));
			else return string.Format("{0:N1} MB", size / (1000.0 * 1024));
		}
	}
}