using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Web.Code;
using System.Data;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using Common;
using Common.Models;
using System.Text.RegularExpressions;

namespace Web.Controllers
{
	[Authorize]
    public class GlossaryController : Controller
    {
        public ActionResult Term(int? id, string term)
        {
			var m = new GlossaryView();

			//m.Terms = DataAccess.GetAllTerms();
			if (id.HasValue)
			{
				m.ActiveTerm = DataAccess.GetSingleTerm(id.Value);
			}
			else if (!string.IsNullOrEmpty(term))
			{
				m.ActiveTerm = DataAccess.GetSingleTerm(term);
			}

			m.Version = DataAccess.GetKeyValue("GlossaryVersion");

            return View("Term",m);
        }

		public FileResult Print(int? id)
		{
			var terms = new List<GlossaryItem>();
			string name = "Glossary.pdf";
			if (id.HasValue)
			{
				var term = DataAccess.GetSingleTerm(id.Value);
				name = string.Format("{0}.pdf", term.Term);
				terms.Add(term);
			}
			else
			{
				terms = DataAccess.GetAllTermsFull().ToList();
			}
			var m = PrintTerms(terms);
			return File(m, "application/pdf", name);
		}

		private MemoryStream PrintTerms(IEnumerable<GlossaryItem> terms)
		{
			float ppi = 72.0F;
			float PageWidth = 8.5F;
			float PageHeight = 11.0F;
			float TopBottomMargin = 0.625F;
			float LeftRightMargin = 0.75F;
			float GutterWidth = 0.25F;
			float HeaderFooterHeight = 0.125F;
			float Column1Left = LeftRightMargin;
			float Column1Right = ((PageWidth - (LeftRightMargin * 2) - GutterWidth) / 2) + LeftRightMargin;
			float Column2Left = Column1Right + GutterWidth;
			float Column2Right = PageWidth - LeftRightMargin;

			bool HasMultipleDefinitions = (terms.Count() > 1);

			string version = DataAccess.GetKeyValue("glossaryversion").Value;

			Document doc = new Document(new Rectangle(0, 0, PageWidth * ppi, PageHeight * ppi));

			MemoryStream m = new MemoryStream();

			PdfWriter writer = PdfWriter.GetInstance(doc, m);
			writer.CloseStream = false;

			try {
				doc.AddTitle("The Glossary of Systematic Christian Theology");
				doc.AddSubject("A collection of technical terms and associated definitions as taught from the pulpit by Dr. Ron Killingsworth in classes at Rephidim Church.");
				doc.AddCreator("My program using iText#");
				doc.AddAuthor("Dr. Ron Killingsworth");
				doc.AddCreationDate();

				writer.SetEncryption(PdfWriter.STRENGTH128BITS, null, "xaris", PdfWriter.AllowCopy | PdfWriter.AllowPrinting | PdfWriter.AllowScreenReaders | PdfWriter.AllowDegradedPrinting);

				doc.Open();

				BaseFont bfArialBlack = BaseFont.CreateFont("C:\\windows\\fonts\\ariblk.ttf", BaseFont.WINANSI, false);
				BaseFont bfGaramond = BaseFont.CreateFont("C:\\windows\\fonts\\gara.ttf", BaseFont.WINANSI, false);
				Font fntTerm = new Font(bfArialBlack, 10, Font.BOLD, new BaseColor(57, 81, 145));
				Font fntDefinition = new Font(bfGaramond, 9, Font.NORMAL, new BaseColor(0, 0, 0));

				///////////////////////////////////////////


				if (HasMultipleDefinitions) {
					PdfContentByte cbCover = new PdfContentByte(writer);
					cbCover = writer.DirectContent;

					cbCover.BeginText();

					cbCover.SetFontAndSize(bfGaramond, 42);
					cbCover.SetRGBColorFill(57, 81, 145);
					cbCover.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Glossary of", (PageWidth / 2) * ppi, ((PageHeight / 2) + Convert.ToSingle(1.5)) * ppi, 0);
					cbCover.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Systematic Christian Theology", (PageWidth / 2) * ppi, ((PageHeight / 2) + 1) * ppi, 0);

					cbCover.SetFontAndSize(bfGaramond, 16);
					cbCover.SetRGBColorFill(0, 0, 0);
					cbCover.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "A collection of technical terms and associated definitions", (PageWidth / 2) * ppi, ((PageHeight / 2) + Convert.ToSingle(0.6)) * ppi, 0);
					cbCover.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "as taught by Dr. Ron Killingsworth, Rephidim Church, Wichita Falls, Texas", (PageWidth / 2) * ppi, ((PageHeight / 2) + Convert.ToSingle(0.4)) * ppi, 0);

					cbCover.SetFontAndSize(bfGaramond, 12);
					cbCover.SetRGBColorFill(0, 0, 0);
					cbCover.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Published by:", (PageWidth / 2) * ppi, ((PageHeight / 2) - Convert.ToSingle(0.6)) * ppi, 0);
					cbCover.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "Rephidim Doctrinal Bible Studies, Inc.", (PageWidth / 2) * ppi, ((PageHeight / 2) - Convert.ToSingle(0.8)) * ppi, 0);
					cbCover.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "4430 Allendale Rd., Wichita Falls, Texas 76310", (PageWidth / 2) * ppi, ((PageHeight / 2) - Convert.ToSingle(1.0)) * ppi, 0);
					cbCover.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "(940) 691-1166     rephidim.org     rephidimwf@gmail.com", (PageWidth / 2) * ppi, ((PageHeight / 2) - Convert.ToSingle(1.2)) * ppi, 0);

					cbCover.SetFontAndSize(bfGaramond, 10);
					cbCover.SetRGBColorFill(0, 0, 0);
					cbCover.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Version: " + version + " / " + terms.Count() + " terms", (LeftRightMargin) * ppi, (TopBottomMargin) * ppi, 0);

					cbCover.SetFontAndSize(bfGaramond, 10);
					cbCover.SetRGBColorFill(0, 0, 0);
					cbCover.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "© 1971-" + DateTime.Today.Year.ToString() + " Dr. Ron Killingsworth, Rephidim Doctrinal Bible Studies, Inc.", (PageWidth - LeftRightMargin) * ppi, (TopBottomMargin) * ppi, 0);


					cbCover.EndText();

					doc.NewPage();

					///////////////////////////////////////////

					PdfContentByte cbBlank = new PdfContentByte(writer);
					cbBlank = writer.DirectContent;

					cbBlank.BeginText();

					cbCover.SetFontAndSize(bfGaramond, 10);
					cbCover.SetRGBColorFill(0, 0, 0);
					cbCover.ShowTextAligned(PdfContentByte.ALIGN_LEFT, " ", (LeftRightMargin) * ppi, (TopBottomMargin) * ppi, 0);

					cbBlank.EndText();

					doc.NewPage();

					///////////////////////////////////////////

				}

				PdfContentByte cb = writer.DirectContent;
				ColumnText ct = new ColumnText(cb);

				int[] left = {
					Convert.ToInt32(Column1Left * ppi),
					Convert.ToInt32(Column2Left * ppi)
				};
				int[] right = {
					Convert.ToInt32(Column1Right * ppi),
					Convert.ToInt32(Column2Right * ppi)
				};

				foreach (var term in terms) {
					ct.AddText(new Phrase(term.Term.Trim() + Environment.NewLine, fntTerm));
					ct.AddText(new Phrase(Regex.Replace(term.Definition.Trim().Replace("<br/>", "\r"), @"<[^>]+>", "") + Environment.NewLine + Environment.NewLine, fntDefinition));
				}

				int status = 0;
				int column = 0;
				int PageNumber = 0;
				if (HasMultipleDefinitions) {
					PageNumber = 3;
				} else {
					PageNumber = 1;
				}
				while ((status & ColumnText.NO_MORE_TEXT) == 0) {
					///////////////////////////////////////////


					PdfContentByte cbPage = new PdfContentByte(writer);
					cbPage = writer.DirectContent;

					cbPage.SetLineWidth(0.5f);
					cbPage.SetRGBColorStroke(50, 50, 50);
					cbPage.MoveTo(LeftRightMargin * ppi, (PageHeight - TopBottomMargin - (HeaderFooterHeight / 2)) * ppi);
					cbPage.LineTo((PageWidth - LeftRightMargin) * ppi, (PageHeight - TopBottomMargin - (HeaderFooterHeight / 2)) * ppi);
					cbPage.Stroke();

					cbPage.SetLineWidth(0.5f);
					cbPage.SetRGBColorStroke(50, 50, 50);
					cbPage.MoveTo(LeftRightMargin * ppi, (TopBottomMargin + HeaderFooterHeight) * ppi);
					cbPage.LineTo((PageWidth - LeftRightMargin) * ppi, (TopBottomMargin + HeaderFooterHeight) * ppi);
					cbPage.Stroke();

					cbPage.BeginText();

					cbPage.SetFontAndSize(bfGaramond, 10);
					cbPage.SetRGBColorFill(0, 0, 0);
					cbPage.ShowTextAligned(PdfContentByte.ALIGN_CENTER, "The Glossary of Systematic Christian Theology", (PageWidth / 2) * ppi, (PageHeight - TopBottomMargin) * ppi, 0);

					if (PageNumber % 2 == 0) {
						cbPage.SetFontAndSize(bfGaramond, 8);
						cbPage.SetRGBColorFill(0, 0, 0);
						cbPage.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "© 1971-" + DateTime.Today.Year.ToString() + " Dr. Ron Killingsworth, Rephidim Doctrinal Bible Studies, Inc.", (PageWidth - LeftRightMargin) * ppi, (TopBottomMargin) * ppi, 0);

						cbPage.SetFontAndSize(bfGaramond, 8);
						cbPage.SetRGBColorFill(0, 0, 0);
						cbPage.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Page " + PageNumber.ToString(), (LeftRightMargin) * ppi, (TopBottomMargin) * ppi, 0);
					} else {
						cbPage.SetFontAndSize(bfGaramond, 8);
						cbPage.SetRGBColorFill(0, 0, 0);
						cbPage.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "© 1971-" + DateTime.Today.Year.ToString() + " Dr. Ron Killingsworth, Rephidim Doctrinal Bible Studies, Inc.", (LeftRightMargin) * ppi, (TopBottomMargin) * ppi, 0);

						cbPage.SetFontAndSize(bfGaramond, 8);
						cbPage.SetRGBColorFill(0, 0, 0);
						cbPage.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Page " + PageNumber.ToString(), (PageWidth - LeftRightMargin) * ppi, (TopBottomMargin) * ppi, 0);

					}

					cbPage.EndText();


					///////////////////////////////////////////


					//If HasMultipleDefinitions Then
					//    ct.setSimpleColumn(left(column), (TopBottomMargin + (HeaderFooterHeight * 3 / 2)) * ppi, right(column), (PageHeight - TopBottomMargin - HeaderFooterHeight) * ppi, 10, Element.ALIGN_LEFT)
					//Else
					//    ct.setSimpleColumn(LeftRightMargin * ppi, (TopBottomMargin + (HeaderFooterHeight * 3 / 2)) * ppi, (PageWidth - LeftRightMargin) * ppi, (PageHeight - TopBottomMargin - HeaderFooterHeight) * ppi, 10, Element.ALIGN_LEFT)
					//End If
					//status = ct.go()
					//If (status And ColumnText.NO_MORE_COLUMN) <> 0 Then
					//    column += 1
					//    If column > 1 Then

					//        doc.newPage()
					//        PageNumber += 1
					//        column = 0
					//    End If
					//End If

					if (HasMultipleDefinitions) {
						ct.SetSimpleColumn(left[column], (TopBottomMargin + (HeaderFooterHeight * 3 / 2)) * ppi, right[column], (PageHeight - TopBottomMargin - HeaderFooterHeight) * ppi, 10, Element.ALIGN_LEFT);

						status = ct.Go();
						if ((status & ColumnText.NO_MORE_COLUMN) != 0) {
							column += 1;

							if (column > 1) {
								doc.NewPage();
								PageNumber += 1;
								column = 0;
							}
						}
					} else {
						ct.SetSimpleColumn(LeftRightMargin * ppi, (TopBottomMargin + (HeaderFooterHeight * 3 / 2)) * ppi, (PageWidth - LeftRightMargin) * ppi, (PageHeight - TopBottomMargin - HeaderFooterHeight) * ppi, 10, Element.ALIGN_LEFT);

						status = ct.Go();
						if ((status) != 0) {
							doc.NewPage();
							PageNumber += 1;
						}
					}

				}


				doc.Close();
				m.Flush();
				m.Position = 0;
				return m;

			} 
			catch (Exception ex) 
			{
				throw ex;
			} 
		}


    }
}
