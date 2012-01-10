/// <reference path="app.js" />
/// <reference path="filehelper.js" />
/// <reference path="../libs/Class.js" />

(function ()
{

	var _firstLoad = true, _books, _terms;

	var domSetup = function (me)
	{
		$(".book-link").live("click", function ()
		{
			var lnk = $(this);
			var details = lnk.data("details").split("|");

			var book = {
				Id: details[0],
				Name: details[1],
				Chapters: details[2]
			};

			$.ajax(
			{
				url: App.ResolveUrl("~/Ajax/Scripture/Verses"),
				data: { bookId: book.Id, chapterNum: 1 },
				success: function (data)
				{
					var all = {
						EditRights: true,
						ActiveBook: book,
						ActiveChapter: 1,
						Verses: data
					};

					$("#tmpBookContent").tmpl(all).appendTo($("#uxContents").empty());

					$(window).resize();
					$("#rangeChapter").change();

					// \\b gets the word boundaries so we only get full words
					// i gets case insensitive
					// g makes it global and not just first
					var regex = new RegExp("\\b(" + _terms.replace(/(\^|\.|\*|\+|\?|\=|\!|\\|\/|\(|\)|\[|\]|\{|\})/ig, "\\$1") + ")\\b", "ig");
					$("#uxVerses").html($("#uxVerses").html().replace(regex, "<a class='term-link inactive-link' data-value='$1' href='#'>$1</a>")); // + pad(0);

					setTimeout(function () { $("#uxVerses a").switchClass("inactive-link", "active-link", "slow"); }, 0);
				},
				error: function (xhr)
				{
					App.HandleError(xhr);
				}
			});
		});

		$("#rangeChapter").live("change", function ()
		{
			var rng = $(this);
			$("#uxChapter").text(rng.val());
		});

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

			$.ajax(
			{
				url: App.ResolveUrl("~/Ajax/Glossary/List"),
				success: function (data)
				{
					_terms = $.map(data, function (n) { return n.Term; }).join("|");
				},
				error: function (xhr)
				{
					log(xhr)
				}
			});
		}

	});

}).call(this);