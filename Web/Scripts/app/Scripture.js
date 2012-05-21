/// <reference path="app.js" />
/// <reference path="glossaryhelper.js" />
/// <reference path="scripturehelper.js" />
/// <reference path="../libs/Class.js" />

(function ()
{

	var _firstLoad = true, _bookName, _book, _chapter, _verse, _timer;

	var loadChapter = function (chapterNum, verseNum, push)
	{
		chapterNum = chapterNum || 1;
		verseNum = verseNum || 1;
		$.ajax(
		{
			url: App.ResolveUrl("~/Ajax/Scripture/Verses"),
			data: { bookId: _book.Id, chapterNum: chapterNum },
			success: function (data)
			{
				$("#uxChapter").text(chapterNum);
				$(".report-issue").data("details", "Book: " + _book.Name + ", Chapter: " + chapterNum);
				$("#tmpChapterContent").tmpl({ Verses: data }).appendTo($("#uxVerses").empty());
				$(window).resize();
				App.GlossaryHelper.HighlightTerms("#uxVerses");
				if (verseNum != null && verseNum != 1)
					setTimeout(function () { $("#uxVerses").scrollTo("." + verseNum, 1000, { easing: 'swing', axis: 'y' }); }, 1000);

				if (_book != null)
				{
					App.SetTitle(_book.Name + " " + chapterNum + ":" + verseNum);
					if (Modernizr.history && push)
						history.pushState({ Book: _book.Name, Chapter: chapterNum, Verse: verseNum }, _book.Name + " " + chapterNum + ":" + verseNum, App.ResolveUrl("~/Scripture/" + _book.Name + "/" + chapterNum + "/" + verseNum));
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
					click: saveVerse,
					"class": "btn btn-primary"
				},
				{
					text: "Cancel",
					click: function () { $(this).dialog("close"); },
					"class": "btn"
				}
			]
		});
	};

	var domSetup = function (me)
	{

		window.onload = function ()
		{
			_firstLoad = true;
			_bookName = _bookName || "Genesis";
			_chapter = _chapter || 1;
			_verse = _verse || 1;

			if (Modernizr.history) history.replaceState({ Book: _bookName, Chapter: _chapter, Verse: _verse }, _bookName + " " + _chapter + ":" + _verse, App.ResolveUrl("~/Scripture/" + _bookName + "/" + _chapter + "/" + _verse));

			App.ScriptureHelper.LoadBooks(function (books)
			{
				if (_bookName != null)
				{
					var matches = $.grep(books, function (n) { return n.Name.toUpperCase() == _bookName.toUpperCase() || ("|" + n.Aliases.toUpperCase() + "|").indexOf("|" + _bookName.toUpperCase() + "|") >= 0; });
					if (matches.length > 0)
					{
						_book = matches[0];
					}
				}
				loadBook(_chapter, _verse, false);
			});

			setTimeout(function () { _firstLoad = false; }, 0);
		};

		if (Modernizr.history)
		{
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
								var matches = $.grep(books, function (n) { return n.Name.toUpperCase() == event.state.Book.toUpperCase() || ("|" + n.Aliases.toUpperCase() + "|").indexOf("|" + _bookName.toUpperCase() + "|") >= 0; });
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

		$(document)

		.on("click", ".book-link", function ()
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
		})

		.on("change", "#rangeChapter, #cmbChapter", function ()
		{
			var chapter = $(this).val();
			clearTimeout(_timer);
			_timer = setTimeout(function ()
			{
				loadChapter(chapter, null, true);
			}, 750);
		})

		.on("click", ".verse-edit", function ()
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