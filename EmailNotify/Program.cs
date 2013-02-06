using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Configuration;
using Postal;
using System.Web.Mvc;
using System.Net.Mail;
using Common.Models;

namespace EmailNotify
{
	class Program
	{
		static void Main(string[] args)
		{
			bool quietMode = false;
			foreach (var arg in args)
			{
				if (arg.ToUpperInvariant().Contains("Q")) quietMode = true;
			}

			try
			{

				Console.WriteLine("Program starting" + (quietMode ? " in quiet mode" : "") + "....");
				DateTime lastDate = DateTime.Now.AddDays(-Convert.ToInt32(ConfigurationManager.AppSettings["days"]));
				var storedDate = DataAccess.GetKeyValue("lastemail").Value;
				if (!string.IsNullOrEmpty(storedDate))
				{
					lastDate = Convert.ToDateTime(storedDate);
				}
				Console.WriteLine("Last Run: {0:M/d/yyyy h:mm tt}", lastDate);
				Console.WriteLine("Looking for verses....");
				var verses = DataAccess.GetRecentVerses(lastDate);
				Console.WriteLine("Looking for terms....");
				var terms = DataAccess.GetRecentTerms(lastDate);
				Console.WriteLine("Looking for files....");
				var files = FileUtility.Recent(lastDate);
				Console.WriteLine("Looking for email addresses....");
				var emails = DataAccess.GetEmails();

				verses = (verses != null ? verses.Take(50) : new List<ScriptureItem>());
				terms = (terms != null ? terms.Take(50) : new List<GlossaryItem>());
				files = (files != null ? files.Take(50) : new List<FileInfoResult>());

				bool sendEmail = ((verses.Count() > 0 || terms.Count() > 0 || files.Count() > 0) && emails != null && emails.Count() > 0);

				if (!sendEmail)
				{
					Console.WriteLine();
					if (!quietMode)
					{
						Console.WriteLine("Nothing new. Not sending email. Press any key to exit.");
						Console.Read();
					}
					else
					{
						Console.WriteLine("Nothing new. Not sending email.");
					}
					Environment.Exit(0);
				}

				Console.WriteLine("Preparing email....");
				var viewsPath = ConfigurationManager.AppSettings["TemplatePath"];

				var engines = new ViewEngineCollection();
				var razorEngine = new FileSystemRazorViewEngine(viewsPath);
				engines.Add(razorEngine);

				var service = new EmailService(engines);

				NotifyEmail email = new NotifyEmail("Notification");
				var webroot = ConfigurationManager.AppSettings["AbsoluteRoot"];
				email.AbsoluteRoot = webroot.EndsWith("/") ? webroot.Substring(0, webroot.Length - 1) : webroot;
				email.To = string.Join(", ", emails.Select(x=>x.Email).ToArray());
				email.Verses = verses;
				email.Terms = terms;
				email.Files = files;

				bool withAttachments = false;
				if (files != null)
				{
					long limit = Convert.ToInt64(ConfigurationManager.AppSettings["AttachmentLimitInMB"]) * 1000 * 1024;
					if (files.Sum(x => x.Length) <= limit)
					{
						withAttachments = true;
						foreach (var file in files)
						{
							var root = ConfigurationManager.AppSettings["FilesRoot"];
							if (root.EndsWith("/")) root = root.Remove(root.Length - 1);
							var fullName = file.Path.Replace("/", @"\");
							if (!fullName.StartsWith(@"\")) fullName = @"\" + fullName;
							fullName = root + fullName;

							email.Attach(new Attachment(fullName));
						}
					}
				}
				email.HasAttachments = withAttachments;
				service.Send(email);

				var newDate = DateTime.Now.ToString("M/d/yyyy h:mm tt");
				DataAccess.UpdateKeyValue("lastemail", newDate);

				Console.WriteLine();
				Console.WriteLine("Sent an email to {0} addresses with {1} verses, {2} terms, {3} files ({4}) and updated the date for next time.", emails.Count(), verses.Count(), terms.Count(), files.Count(), withAttachments ? "with attachments" : "no attachments");
			
				Console.WriteLine();
				if (!quietMode)
				{
					Console.WriteLine("Press any key to exit.");
					Console.Read();
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				if (!quietMode)
				{
					Console.WriteLine("Press any key to exit.");
					Console.Read();
				}
			}
		}
	}
}
