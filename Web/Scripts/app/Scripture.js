/// <reference path="app.js" />
/// <reference path="glossaryhelper.js" />
/// <reference path="../libs/Class.js" />

(function ()
{

	var _firstLoad = true, _books, _terms, _book;

	var loadChapter = function (chapterNum)
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
			},
			error: function (xhr)
			{
				App.HandleError(xhr);
			}
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


			var chapters = [];
			for (var i = 0; i < _book.Chapters; i++)
			{
				chapters.push({ Chapter: i + 1 });
			}

			var all = {
				EditRights: true,
				ActiveBook: _book,
				ActiveChapter: 1,
				Chapters: chapters
			};

			$("#tmpBookContent").tmpl(all).appendTo($("#uxContents").empty());

			loadChapter(1);
		});

		$("#rangeChapter, #cmbChapter").live("change", $.debounce(750, function ()
		{
			var rng = $(this);
			loadChapter(rng.val());
		}));

		//		if (Modernizr.history)
		//		{
		//			window.onload = function ()
		//			{
		//				_firstLoad = true;

		//				if (_browsePath != "")
		//				{
		//					history.replaceState({ Path: _browsePath }, _browsePath, App.ResolveUrl("~/Files/Browse/" + _browsePath));
		//				}

		//				loadContents(_browsePath, false);
		//				setTimeout(function () { _firstLoad = false; }, 0);
		//			};

		//			window.onpopstate = function (event)
		//			{
		//				if (_firstLoad)
		//				{
		//					_firstLoad = false;
		//				}
		//				else
		//				{
		//					if (event.state != null)
		//					{
		//						loadContents(event.state.Path, false);
		//					}
		//					else
		//					{
		//						loadContents("", false);
		//					}
		//				}

		//			};
		//		}


		//		$(".folder-expand").live("click", function ()
		//		{
		//			$(".popover").fadeOut();
		//			var a = $(this);

		//			if (a.hasClass("ui-icon-plus"))
		//			{
		//				loadFolders(a.data("path"), function (data)
		//				{
		//					a.removeClass("ui-icon-plus").addClass("ui-icon-minus");
		//					$("#tmpFolders").tmpl(data).appendTo($("<ul class='folders'></ul>").appendTo(a.parent("li")));
		//				});
		//			}
		//			else
		//			{
		//				a.parent("li").find("ul").remove();
		//				a.removeClass("ui-icon-minus").addClass("ui-icon-plus");
		//			}

		//			return false;
		//		});

		//		$(".folder-view").live("click", function ()
		//		{
		//			$(".popover").fadeOut();
		//			var a = $(this);

		//			var path = a.data("path");

		//			if (a.siblings(".folder-expand").hasClass("ui-icon-plus"))
		//			{
		//				loadFolders(path, function (data)
		//				{
		//					a.siblings(".folder-expand").removeClass("ui-icon-plus").addClass("ui-icon-minus");
		//					$("#tmpFolders").tmpl(data).appendTo($("<ul class='folders'></ul>").appendTo(a.parent("li")));
		//				});
		//			}
		//			else
		//			{
		//				a.parent("li").find("ul").remove();
		//				a.siblings(".folder-expand").removeClass("ui-icon-minus").addClass("ui-icon-plus");
		//			}

		//			loadContents(path, true);

		//			return false;
		//		});

		$(window).resize($.throttle(250, function ()
		{
			$("#uxBooks").height($(window).height() - 60);
			$("#uxContents").height($(window).height() - 60);

		})).resize();


	};

	this.App.Scripture = Class.extend(
	{
		init: function (books)
		{
			$("#tmpBookList").tmpl(books).appendTo("#ulBooks");

			_editRights = true;

			//			_browsePath = browsePath;
			//			
			//			loadFolders("", function (data)
			//			{
			//				$("#tmpFolders").tmpl(data).appendTo("#ulFolders");
			//			});

			//			App.FileHelper.ReadyPopover();

			domSetup(this);
		}

	});

}).call(this);