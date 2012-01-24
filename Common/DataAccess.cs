using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Data.SqlServerCe;
using System.Configuration;
using Common.Models;
using System.Collections;
using System.Text.RegularExpressions;

namespace Common
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
		public static IEnumerable<GlossaryItem> GetRecentTerms()
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();
					p.Add("@days", ConfigurationManager.AppSettings["days"]);

					var res = cn.Query("SELECT TermId, Term, SUBSTRING(Definition,0, 125) + '...' as Definition, DateCreated, DateModified, CONVERT(bit,CASE WHEN GETDATE() < DATEADD(day,@days,DateModified) THEN 1 ELSE 0 END) AS IsModified, CONVERT(bit,CASE WHEN GETDATE() < DATEADD(day,@days,DateCreated) THEN 1 ELSE 0 END) AS IsNew  FROM GlossaryTerms WHERE GETDATE() < DATEADD(day,@days,DateModified) OR GETDATE() < DATEADD(day,@days,DateCreated)", p);

					if (res == null) return null;

					return res.Select(x => new GlossaryItem
						{
							Id = x.TermId,
							Term = x.Term,
							Definition = x.Definition,
							DateCreated = x.DateCreated,
							DateModified = x.DateModified,
							IsNew = x.IsNew,
							IsModified = x.IsModified
						});
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static IEnumerable<BookItem> GetBooks()
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();

					var res = cn.Query("SELECT BookId, BookName, Chapters FROM Books ORDER BY BookId", p);

					if (res == null) return null;

					return res.Select(x => new BookItem
					{
						Id = x.BookId,
						Name = x.BookName,
						Chapters = x.Chapters
					});
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static IEnumerable<ScriptureItem> GetVerses(int bookId, int chapterNum)
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();
					p.Add("@days", ConfigurationManager.AppSettings["days"]);
					p.Add("@bookId", bookId);
					p.Add("@chapterNum", chapterNum);

					var res = cn.Query("SELECT ScriptureId, Scriptures.BookId, BookName, Chapters, ChapterNum, VerseNum, VerseText, Notes, DateCreated, DateModified, CONVERT(bit,CASE WHEN GETDATE() < DATEADD(day,@days,DateModified) THEN 1 ELSE 0 END) AS IsModified, CONVERT(bit,CASE WHEN GETDATE() < DATEADD(day,@days,DateCreated) THEN 1 ELSE 0 END) AS IsNew FROM Scriptures INNER JOIN Books ON Books.BookId=Scriptures.BookId WHERE Scriptures.BookId=@bookId AND ChapterNum=@chapterNum ORDER BY Books.BookId, ChapterNum, VerseNum", p);

					if (res == null) return null;

					return res.Select(x => new ScriptureItem
					{
						Id = x.ScriptureId,
						Book = new BookItem {Id = x.BookId, Name =x.BookName, Chapters = x.Chapters},
						Chapter = x.ChapterNum,
						Verse = x.VerseNum,
						Text = x.VerseText,
						Notes = x.Notes,
						IsNew = x.IsNew,
						IsModified = x.IsModified,
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

		public static IEnumerable<GlossaryItem> GetAllTermsFull()
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();
					p.Add("@days", ConfigurationManager.AppSettings["days"]);

					var res = cn.Query("SELECT TermId, Term, Definition, DateCreated, DateModified, CONVERT(bit,CASE WHEN GETDATE() < DATEADD(day,@days,DateModified) THEN 1 ELSE 0 END) AS IsModified, CONVERT(bit,CASE WHEN GETDATE() < DATEADD(day,@days,DateCreated) THEN 1 ELSE 0 END) AS IsNew  FROM GlossaryTerms ORDER BY Term", p);

					if (res == null) return null;

					return res.Select(x => new GlossaryItem
						{
							Id = x.TermId,
							Term = x.Term,
							Definition = x.Definition,
							DateCreated = x.DateCreated,
							DateModified = x.DateModified,
							IsModified = x.IsModified,
							IsNew = x.IsNew
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
					p.Add("@days", ConfigurationManager.AppSettings["days"]);

					var res = cn.Query("SELECT TermId, Term, DateCreated, DateModified, CONVERT(bit,CASE WHEN GETDATE() < DATEADD(day,@days,DateModified) THEN 1 ELSE 0 END) AS IsModified, CONVERT(bit,CASE WHEN GETDATE() < DATEADD(day,@days,DateCreated) THEN 1 ELSE 0 END) AS IsNew  FROM GlossaryTerms ORDER BY Term", p);

					if (res == null) return null;

					return res.Select(x => x.IsNew || x.IsModified
						? new GlossarySummaryMore
						{
							Id = x.TermId,
							Term = x.Term,
							DateCreated = x.DateCreated,
							DateModified = x.DateModified,
							IsModified = x.IsModified,
							IsNew = x.IsNew
						}
						: new GlossarySummary
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
					p.Add("@days", ConfigurationManager.AppSettings["days"]);

					var res = cn.Query("SELECT TermId, Term, Definition, DateCreated, DateModified, CONVERT(bit,CASE WHEN GETDATE() < DATEADD(day,@days,DateModified) THEN 1 ELSE 0 END) AS IsModified, CONVERT(bit,CASE WHEN GETDATE() < DATEADD(day,@days,DateCreated) THEN 1 ELSE 0 END) AS IsNew FROM GlossaryTerms WHERE TermId=@termid", p);

					if (res == null) return null;

					return res.Select(x => new GlossaryItem
					{
						Id = x.TermId,
						Term = x.Term,
						Definition = x.Definition,
						DateCreated = x.DateCreated,
						DateModified = x.DateModified,
						IsModified = x.IsModified,
						IsNew = x.IsNew
					}).FirstOrDefault();
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static GlossaryItem UpsertTerm(int id, string term, string definition, bool? updateDate)
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();
					p.Add("@termid", id);
					p.Add("@term", term.Trim().ToUpperInvariant());
					p.Add("@definition", Regex.Replace(definition.Trim(), "(\r|\n)", "<br/>", RegexOptions.IgnoreCase));
					p.Add("@datemodified", DateTime.Now);
					p.Add("@days", ConfigurationManager.AppSettings["days"]);

					string lastPart = "";
					string firstquery = "";
					string secondquery = "";
					if (id > 0)
					{
						// edit
						if (updateDate.HasValue && updateDate.Value)
						{
							firstquery = "UPDATE GlossaryTerms SET Term=@Term, Definition=@Definition, DateModified=@DateModified WHERE TermID=@termid; ";
						}
						else
						{
							firstquery = "UPDATE GlossaryTerms SET Term=@Term, Definition=@Definition WHERE TermID=@termid; ";
						}
						lastPart = "@termid";
					}
					else
					{
						// insert
						firstquery = "INSERT GlossaryTerms(Term, Definition) VALUES (@Term, @Definition);";
						lastPart = "@@IDENTITY";
					}

					secondquery = "SELECT TermId, Term, Definition, DateCreated, DateModified, CONVERT(bit,CASE WHEN GETDATE() < DATEADD(day,@days,DateModified) THEN 1 ELSE 0 END) AS IsModified, CONVERT(bit,CASE WHEN GETDATE() < DATEADD(day,@days,DateCreated) THEN 1 ELSE 0 END) AS IsNew FROM GlossaryTerms WHERE TermId=" + lastPart;

					var exec = cn.Execute(firstquery, p);
					var res = cn.Query(secondquery, p);

					if (res == null) return null;

					return res.Select(x => new GlossaryItem
					{
						Id = x.TermId,
						Term = x.Term,
						Definition = x.Definition,
						DateCreated = x.DateCreated,
						DateModified = x.DateModified,
						IsModified = x.IsModified,
						IsNew = x.IsNew
					}).FirstOrDefault();
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static int DeleteTerm(int id)
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();
					p.Add("@termid", id);
					
					var exec = cn.Execute("DELETE FROM GLossaryTerms WHERE TermId=@termid", p);

					return exec;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static IEnumerable<GlossaryItem> SearchTerms(string[] parts)
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();
					var p = new DynamicParameters();
					p.Add("@days", ConfigurationManager.AppSettings["days"]);

					var res = cn.Query(CreateSearchQuery(parts), p);

					if (res == null) return null;

					return res.Select(x => new GlossaryItem
					{
						Id = x.TermId,
						Term = x.Term,
						Definition = SliceRelevantChunk(x.Definition, parts),
						DateCreated = x.DateCreated,
						DateModified = x.DateModified,
						IsNew = x.IsNew,
						IsModified = x.IsModified

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

			string SqlString = "SELECT TermId, Term, Definition, DateCreated, DateModified, CONVERT(bit,CASE WHEN GETDATE() < DATEADD(day,@days,DateModified) THEN 1 ELSE 0 END) AS IsModified, CONVERT(bit,CASE WHEN GETDATE() < DATEADD(day,@days,DateCreated) THEN 1 ELSE 0 END) AS IsNew FROM GlossaryTerms WHERE ";

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

					res += "..." + definition.Substring(index - left, index + right - (index - left)) + "... &nbsp;";
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
					p.Add("@days", ConfigurationManager.AppSettings["days"]);

					var res = cn.Query("SELECT TermId, Term, Definition, DateCreated, DateModified, CONVERT(bit,CASE WHEN GETDATE() < DATEADD(day,@days,DateModified) THEN 1 ELSE 0 END) AS IsModified, CONVERT(bit,CASE WHEN GETDATE() < DATEADD(day,@days,DateCreated) THEN 1 ELSE 0 END) AS IsNew FROM GlossaryTerms WHERE Term=@term", p);

					if (res == null) return null;

					return res.Select(x => new GlossaryItem
					{
						Id = x.TermId,
						Term = x.Term,
						Definition = x.Definition,
						DateCreated = x.DateCreated,
						DateModified = x.DateModified,
						IsNew = x.IsNew,
						IsModified = x.IsModified
					}).FirstOrDefault();
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static KeyValuePair<string,string> GetKeyValue(string key)
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();
					p.Add("@key", key);

					var res = cn.Query("SELECT KeyName, ValueString FROM KeysTable WHERE KeyName=@key", p);

					if (res == null)
						return new KeyValuePair<string, string>(key, "");

					return res.Select(x => new KeyValuePair<string,string>(x.KeyName,x.ValueString)).FirstOrDefault();
				}
			}
			catch (Exception)
			{
				return new KeyValuePair<string,string>(key,"");
			}
		}

		public static KeyValuePair<string, string> UpdateKeyValue(string key, string value)
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();
					p.Add("@key", key);
					p.Add("@value", value);

					var res = cn.Execute("UPDATE KeysTable SET ValueString=@Value WHERE KeyName=@key", p);

					if (res == 1)
						return new KeyValuePair<string, string>(key, value);
					else
						throw new Exception("");

				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public static IEnumerable<MessageItem> GetActiveHomeMessages()
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();

					var res = cn.Query("SELECT MessageId, MessageValue, OnLoginPage, OnHomePage, IsActive, Style FROM Messages WHERE IsActive=1 AND OnHomePage=1", p);

					if (res == null) return null;

					return res.Select(x => new MessageItem
					{
						Id = x.MessageId,
						Value = x.MessageValue,
						OnLoginPage = x.OnLoginPage,
						OnHomePage = x.OnHomePage,
						IsActive = x.IsActive,
						Style = x.Style
					});
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static IEnumerable<MessageItem> GetActiveLoginMesssages()
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();

					var res = cn.Query("SELECT MessageId, MessageValue, OnLoginPage, OnHomePage, IsActive, Style FROM Messages WHERE IsActive=1 AND OnLoginPage=1", p);

					if (res == null) return null;

					return res.Select(x => new MessageItem
					{
						Id = x.MessageId,
						Value = x.MessageValue,
						OnLoginPage = x.OnLoginPage,
						OnHomePage = x.OnHomePage,
						IsActive = x.IsActive,
						Style = x.Style
					});
				}
			}
			catch (Exception)
			{
				return null;
			}
		}




		public static int InsertBook(int id, string name)
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();
					p.Add("@BookId", id);
					p.Add("@Name", name);

					var exec = cn.Execute("INSERT nasb_Books(BookId, Name) VALUES(@Bookid, @name);", p);

					return exec;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}


		public static int InsertChapter(int id, int name, int bookId)
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();
					p.Add("@BookId", bookId);
					p.Add("@chapterId", id);
					p.Add("@name", name);

					var exec = cn.Execute("INSERT nasb_Chapters(ChapterId, [Name], BookId) VALUES(@ChapterId, @name, @Bookid);", p);

					return exec;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}


		public static int InsertVerse(int id, int chapterId, string text, int name)
		{
			try
			{
				using (var cn = new SqlCeConnection(ConnString))
				{
					cn.Open();

					var p = new DynamicParameters();
					p.Add("@VerseId", id);
					p.Add("@name", name);
					p.Add("@chapterId", chapterId);
					p.Add("@verseText", text);

					var exec = cn.Execute("INSERT nasb_Verses(VerseId, [Name], ChapterId, VerseText) VALUES(@VerseId, @Name, @ChapterId, @versetext);", p);

					return exec;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

	}
}