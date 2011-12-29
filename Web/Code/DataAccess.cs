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
		public static IEnumerable<GlossaryItem> SearchTerms(string query)
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var res = cn.Query(CreateSearchQuery(query));

					if (res == null) return null;

					return res.Select(x => new GlossaryItem
					{
						Id = x.TermId,
						Term = x.Term,
						Definition = SliceRelevantPart(x.Definition, query),
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

		private static string CreateSearchQuery(string query)
		{
			//Clean it up
			query = query.Replace("--", "").Replace("'", "");

			string[] Parts = query.Split('"');

			ArrayList BoldWords = new ArrayList();

			string SqlString = "SELECT TermId, Term, Definition, DateCreated, DateModified FROM GlossaryTerms WHERE ";
			if (Parts.Length % 2 == 1)
			{
				for (int i = 0; i <= Parts.Length - 1; i++)
				{
					if (!string.IsNullOrEmpty(Parts[i].ToString().Trim()))
					{
						if (i % 2 == 1)
						{
							// this is the part in quotes
							SqlString += "(Term LIKE '%" + Parts[i].ToString().Trim() + "%' OR Definition LIKE '%" + Parts[i].ToString().Trim() + "%') AND ";
							BoldWords.Add(Parts[i].ToString().Trim());
						}
						else
						{
							// this is the part not in quotes
							string[] SmallerParts = Parts[i].ToString().Split(' ');

							foreach (string smallpart in SmallerParts)
							{
								string tempSmallPart = smallpart;
								//remove common words
								string[] CommonWords = ConfigurationManager.AppSettings["CommonWords"].Split(',');

								foreach (string commonword in CommonWords)
								{
									if (smallpart.ToUpper() == commonword.Trim().ToUpper())
									{
										tempSmallPart = "";
									}
								}

								if (!string.IsNullOrEmpty(tempSmallPart.Trim()))
								{
									SqlString += "(Term LIKE '%" + tempSmallPart.Trim() + "%' OR Definition LIKE '%" + tempSmallPart.Trim() + "%') AND ";
									BoldWords.Add(tempSmallPart.Trim());
								}
							}
						}
					}
				}
				// gotta get rid of the " OR "
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
			else
			{
				return "";
			}
		}

		private static string SliceRelevantPart(string definition, string query)
		{
			var index = definition.IndexOf(query, StringComparison.OrdinalIgnoreCase);
			int right = 125;
			int left = 125;
			if (definition.Length - index < right) right = definition.Length - index - 1;
			if (index < left) left = index;

			return Regex.Replace("..." + definition.Substring(index - left, index + right - (index - left)) + "...", @"<[^>]+>"," ");

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