using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Data.SqlServerCe;
using System.Configuration;
using Web.Models;
using System.Collections;
using System.Text.RegularExpressions;

namespace Web.Code
{
	public static class DataAccess
	{
		private static string ConnString = ConfigurationManager.ConnectionStrings["rephidim"].ConnectionString;

		public static UserItem Authenticate(string userName, string password)
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();
					p.Add("@userName", userName);
					p.Add("@password", password);

					var res = cn.Query("SELECT UserId, UserName, Password, Rights FROM Users WHERE UserName=@UserName AND Password=@Password AND IsActive=1", p).FirstOrDefault();

					if (res == null) return null;

					if (res.Password == password)
					{
						return new UserItem
						{
							Id = res.UserId,
							Name = res.UserName,
							Password = res.Password,
							Rights = res.Rights,
							IsActive = true
						};
					}
					else return null;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}
		public static IEnumerable<GlossaryItem> GetRecentTerms(int days)
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();
					p.Add("@days", days);

					var res = cn.Query("SELECT TermId, Term, DateCreated, DateModified FROM GlossaryTerms WHERE GETDATE() < DATEADD(day,@days,DateModified) OR GETDATE() < DATEADD(day,@days,DateCreated)", p);

					if (res == null) return null;

					return res.Select(x => new GlossaryItem
						{
							Id = x.TermId,
							Term = x.Term,
							//Definition = x.Definition,
							DateCreated = x.DateCreated,
							DateModified = x.DateModified
						});
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static IEnumerable<GlossarySummary> GetAllTerms()
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();

					var res = cn.Query("SELECT TermId, Term FROM GlossaryTerms ORDER BY Term", p);

					if (res == null) return null;

					return res.Select(x => new GlossarySummary
					{
						Id = x.TermId,
						Term = x.Term,
					});
				}
			}
			catch (Exception)
			{
				return null;
			}
		}
		public static GlossaryItem GetSingleTerm(int id)
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();
					p.Add("@termid", id);

					var res = cn.Query("SELECT TermId, Term, Definition, DateCreated, DateModified FROM GlossaryTerms WHERE TermId=@termid", p);

					if (res == null) return null;

					return res.Select(x => new GlossaryItem
					{
						Id = x.TermId,
						Term = x.Term,
						Definition = x.Definition,
						DateCreated = x.DateCreated,
						DateModified = x.DateModified
					}).FirstOrDefault();
				}
			}
			catch (Exception)
			{
				return null;
			}
		}
		public static IEnumerable<GlossaryItem> SearchTerms(string[] parts)
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var res = cn.Query(CreateSearchQuery(parts));

					if (res == null) return null;

					return res.Select(x => new GlossaryItem
					{
						Id = x.TermId,
						Term = x.Term,
						Definition = SliceRelevantChunk(x.Definition, parts),
						DateCreated = x.DateCreated,
						DateModified = x.DateModified
					});
				}
			}
			catch (Exception)
			{
				return null;
			}
		}		

		private static string CreateSearchQuery(string[] parts)
		{

			string SqlString = "SELECT TermId, Term, Definition, DateCreated, DateModified FROM GlossaryTerms WHERE ";

			foreach (var part in parts)
			{
				SqlString += "(Term LIKE '%" + part.Trim() + "%' OR Definition LIKE '%" + part.Trim() + "%') AND ";
			}
			
			// gotta get rid of the " AND "
			if (SqlString.Trim().EndsWith("AND"))
			{
				SqlString = SqlString.Remove(SqlString.Length - 4, 4) + " ORDER BY Term";
			}
			else
			{
				SqlString = SqlString + "0=1";
			}

			return SqlString;
		}

		private static string SliceRelevantChunk(string definition, string[] parts)
		{
			definition = Regex.Replace(definition, @"<[^>]+>", " ");
			string res = "";
			int lastIndex = 0;
			int left = 50;
			int right = 75;

			foreach (var part in parts)
			{
				var index = definition.IndexOf(part, StringComparison.OrdinalIgnoreCase);
				if (lastIndex == 0 || (index < lastIndex - left || index > lastIndex + right))
				{
					if (definition.Length - index < right) right = definition.Length - index - 1;
					if (index < left) left = index;

					res += "..." + Regex.Replace(definition.Substring(index - left, index + right - (index - left)), "(" + string.Join("|",parts) + ")", "<span class='hilite'>$1</span>", RegexOptions.IgnoreCase) + "... &nbsp;";
					lastIndex = index;

				}

			}

			return res;


		}

		public static GlossaryItem GetSingleTerm(string term)
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();
					p.Add("@term", term);

					var res = cn.Query("SELECT TermId, Term, Definition, DateCreated, DateModified FROM GlossaryTerms WHERE Term=@term", p);

					if (res == null) return null;

					return res.Select(x => new GlossaryItem
					{
						Id = x.TermId,
						Term = x.Term,
						Definition = x.Definition,
						DateCreated = x.DateCreated,
						DateModified = x.DateModified
					}).FirstOrDefault();
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static VersionItem GetSingleVersion(int id)
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();
					p.Add("@id", id);

					var res = cn.Query("SELECT VersionId, AppName, Number FROM Versions WHERE VersionId=@id", p);

					if (res == null) return null;

					return res.Select(x => new VersionItem
					{
						Id = x.VersionId,
						AppName = x.AppName,
						Number = x.Number
					}).FirstOrDefault();
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static IEnumerable<MessageItem> GetActiveHomeMesssages()
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();

					var res = cn.Query("SELECT MessageId, MessageValue, OnLoginPage, OnHomePage, IsActive FROM Messages WHERE IsActive=1 AND OnHomePage=1", p);

					if (res == null) return null;

					return res.Select(x => new MessageItem
					{
						Id = x.MessageId,
						Value = x.MessageValue,
						OnLoginPage = x.OnLoginPage,
						OnHomePage = x.OnHomePage,
						IsActive = x.IsActive
					});
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

	}
}