/// <reference path="app.js" />
/// <reference path="glossaryhelper.js" />
/// <reference path="scripturehelper.js" />
/// <reference path="../libs/Class.js" />

(function ()
{

	var _firstLoad = true, _bookName, _book, _chapter, _verse;

	var loadChapter = function (chapterNum, verseNum, push)
	{
		$.ajax(
		{
			url: App.ResolveUrl("~/Ajax/Scripture/Verses"),
			data: { bookId: _book.Id, chapterNum: chapterNum },
			success: function (data)
			{
				$("#uxChapter").text(chapterNum);
				$("#tmpChapterContent").tmpl({ Verses: data }).appendTo($("#uxVerses").empty());
				$(window).resize();
				App.GlossaryHelper.HighlightTerms("#uxVerses");
				if (verseNum != null && verseNum != 1)
					setTimeout(function () { $("#uxVerses").scrollTo("." + verseNum, 1000, { easing: 'swing', axis: 'y' }); }, 1000);

				if (Modernizr.history && push)
				{
					if (_book != null && chapterNum != null && verseNum != null)
					{
						history.pushState({ Book: _book.Name, Chapter: chapterNum, Verse: verseNum }, _book.Name + " " + chapterNum + ":" + verseNum, App.ResolveUrl("~/Scripture/" + _book.Name + "/" + chapterNum + "/" + verseNum));
					}
					else if (_bookName != null && _chapter != null)
					{
						history.pushState({ Book: _book.Name, Chapter: chapterNum, Verse: verseNum }, _book.Name + " " + chapterNum, App.ResolveUrl("~/Scripture/" + _book.Name + "/" + chapterNum));
					}
					else if (_bookName != null)
					{
						history.pushState({ Book: _book.Name, Chapter: chapterNum, Verse: verseNum }, _book.Name, App.ResolveUrl("~/Scripture/" + _book.Name));
					}
				}
			},
			error: function (xhr)
			{
				App.HandleError(xhr);
			}
		});
	};

	var loadBook = function (chapter, verse, push)
	{
		if (_book == null)
		{
			var all = {
				EditRights: false,
				ActiveBook: null,
				ActiveChapter: 1,
				Chapters: 1
			};

			$("#tmpBookContent").tmpl(all).appendTo($("#uxContents").empty());
			return;
		};

		chapter = chapter || 1;
		var chapters = [];
		for (var i = 0; i < _book.Chapters; i++)
		{
			chapters.push({ Chapter: i + 1 });
		}

		var all = {
			EditRights: true,
			ActiveBook: _book,
			ActiveChapter: chapter,
			Chapters: chapters
		};
		if (_book != null && _book.Id != 1)
			setTimeout(function () { $("#uxBooks .scrollable").scrollTo("[name='" + _book.Id + "']", 1000, { easing: 'swing', axis: 'y' }); }, 1000);

		$("#tmpBookContent").tmpl(all).appendTo($("#uxContents").empty());

		loadChapter(chapter, verse, push);
	};

	var saveVerse = function ()
	{
		$("#uxVerseDialogError").hide();
		$.ajax(
		{
			url: App.ResolveUrl("~/Ajax/Scripture/Save"),
			type: "POST",
			data: {
				id: $("#hdnVerseEditId").val(),
				text: $("#txtVerseEditText").val(),
				notes: $("#txtVerseEditNotes").val(),
				translationId: $("#txtVerseEditTranslation").val(),
				updateDate: $("#chkVerseEditModify").is(":checked")
			},
			success: function (data)
			{
				loadChapter(data.Chapter, data.Verse, false);
				App.ShowAlert(data.Book.Name + " " + data.Chapter + ":" + data.Verse + " Saved!", "success");
				$("#uxVerseDialog").dialog("close");
			},
			error: function (xhr)
			{
				App.HandleError(xhr, "#uxVerseDialogError");
			}
		});
	};

	var editVerse = function (data)
	{
		$("#tmpVerseEdit").tmpl(data).appendTo($("#uxVerseDialog").empty());
		$("#uxVerseDialog").dialog(
		{
			width: 800,
			height: 500,
			title: data.Book.Name + " " + data.Chapter + ":" + data.Verse,
			modal: true,
			resizable: false,
			buttons: [
				{
					text: "Save",
					click: saveVerse
				},
				{
					text: "Cancel",
					click: function () { $(this).dialog("close"); }
				}
			]
		});
	};

	var domSetup = function (me)
	{
		$(".book-link").live("click", function ()
		{
			var lnk = $(this);
			var details = lnk.data("details").split("|");

			_book = {
				Id: details[0],
				Name: details[1],
				Chapters: details[2]
			};

			loadBook(null, null, true);

			return false;
		});

		$("#rangeChapter, #cmbChapter").live("change", $.debounce(750, function ()
		{
			loadChapter($(this).val(), null, true);
		}));

		$(document).on("click", ".term-link", function ()
		{
			var termName = $(this).data("value");
			App.GlossaryHelper.LoadTerms(function (terms)
			{
				log(terms, termName);
				var matchingTerms = $.grep(terms, function (n) { return n.Term.toUpperCase() == termName.toUpperCase(); });
				if (matchingTerms.length > 0)
				{
					location = App.ResolveUrl("~/Glossary/Term/" + matchingTerms[0].Id);
				}
				else
					location = App.ResolveUrl("~/Glossary");
			});
			return false;

		}).on("mouseenter", ".lines li", function ()
		{
			$(this).find(".edit-btn").show();

		}).on("mouseleave", ".lines li", function ()
		{
			$(this).find(".edit-btn").hide();

		}).on("click", ".edit-btn", function ()
		{
			var lnk = $(this);
			var id = lnk.data("id");

			$.ajax(
			{
				url: App.ResolveUrl("~/Ajax/Scripture/Edit"),
				data: { id: id },
				success: editVerse,
				error: function (xhr)
				{
					App.HandleError(xhr);
				}
			});


			return false;
		});


		if (Modernizr.history)
		{
			window.onload = function ()
			{
				_firstLoad = true;

				if (_bookName != null && _chapter != null && _verse != null)
				{
					history.replaceState({ Book: _bookName, Chapter: _chapter, Verse: _verse }, _bookName + " " + _chapter + ":" + _verse, App.ResolveUrl("~/Scripture/" + _bookName + "/" + _chapter + "/" + _verse));
				}
				else if (_bookName != null && _chapter != null)
				{
					history.replaceState({ Book: _bookName, Chapter: _chapter, Verse: _verse }, _bookName + " " + _chapter, App.ResolveUrl("~/Scripture/" + _bookName + "/" + _chapter));
				}
				else if (_bookName != null)
				{
					history.replaceState({ Book: _bookName, Chapter: _chapter, Verse: _verse }, _bookName, App.ResolveUrl("~/Scripture/" + _bookName));
				}

				App.ScriptureHelper.LoadBooks(function (books)
				{
					if (_bookName != null)
					{
						var matches = $.grep(books, function (n) { return n.Name.toUpperCase() == _bookName.toUpperCase(); });
						if (matches.length > 0)
						{
							_book = matches[0];
						}
					}
					loadBook(_chapter, _verse, false);

				});

				setTimeout(function () { _firstLoad = false; }, 0);
			};

			window.onpopstate = function (event)
			{
				if (_firstLoad)
				{
					_firstLoad = false;
				}
				else
				{
					if (event.state != null)
					{
						App.ScriptureHelper.LoadBooks(function (books)
						{
							if (event.state.Book != null)
							{
								var matches = $.grep(books, function (n) { return n.Name.toUpperCase() == event.state.Book.toUpperCase(); });
								if (matches.length > 0)
								{
									_book = matches[0];
								}
							}

							loadBook(event.state.Chapter, event.state.Verse, false);

						});
					}
					else
					{
						loadBook(null, null, false);
					}
				}

			};
		}

	};

	this.App.Scripture = Class.extend(
	{
		init: function (bookName, chapter, verse)
		{
			_bookName = bookName;
			_chapter = chapter;
			_verse = verse;
			_editRights = true;

			App.ScriptureHelper.LoadBooks(function (books)
			{
				$("#tmpBookList").tmpl({ Books: books }).appendTo("#uxBooks");
			});

			domSetup(this);
		}

	});

}).call(this);