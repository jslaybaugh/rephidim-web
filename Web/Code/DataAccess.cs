using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Data.SqlServerCe;
using System.Configuration;
using Web.Models;

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
		public static IEnumerable<GlossaryItem> SearchTerms(string query)
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();
					p.Add("@query", query);

					var res = cn.Query("SELECT TermId, Term, Definition, DateCreated, DateModified FROM GlossaryTerms WHERE Term LIKE '%' + @query + '%' OR convert(nvarchar(max),Definition) LIKE '%' + @query + '%' ORDER BY Term", p);

					if (res == null) return null;

					return res.Select(x => new GlossaryItem
					{
						Id = x.TermId,
						Term = x.Term,
						Definition = x.Definition,
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
		private static string SliceRelevantPart(string definition, string query)
		{
			var index = definition.IndexOf(query, StringComparison.OrdinalIgnoreCase);
			int right = 125;
			int left = 125;
			if (definition.Length - index < right) right = definition.Length - index;
			if (index < left) left = index;

			return "..." + definition.Substring(index - left, index + right - index - left) + "...";
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