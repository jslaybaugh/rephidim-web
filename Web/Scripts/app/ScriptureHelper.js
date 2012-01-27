/// <reference path="app.js" />
/// <reference path="../libs/json2.js" />
/// <reference path="../libs/consolelog.js" />

(function ()
{
	var _books;

	var getBooks = function (callback)
	{
		if (_books != null)
		{
			callback(_books);
			return;
		}

		if (Modernizr.localstorage)
		{
			var data = localStorage.getItem("books");
			if (data != null)
			{
				var data = JSON.parse(data);
				_books = data.Books;
				callback(_books);
				return;
			}
		}

		$.ajax(
		{
			url: App.ResolveUrl("~/Ajax/Scripture/Books"),
			success: function (data)
			{
				_books = data;
				if (Modernizr.localstorage)
				{
					var obj = { Timestamp: new Date(), Books: _books };
					localStorage.setItem("books", JSON.stringify(obj));
				}

				callback(_books);
			},
			error: function (xhr)
			{
				log(xhr)
			}
		});
	};

	this.App.ScriptureHelper = {

		//		HighlightTerms: function (container)
		//		{
		//			getTerms(function (terms)
		//			{
		//				var termString = $.map(terms, function (n) { return n.Term; }).join("|");

		//				// \\b gets the word boundaries so we only get full words
		//				// i gets case insensitive
		//				// g makes it global and not just first
		//				var regex = new RegExp("\\b(" + termString.replace(/(\^|\.|\*|\+|\?|\=|\!|\\|\/|\(|\)|\[|\]|\{|\})/ig, "\\$1") + ")\\b", "ig");

		//				$(container).html($(container).html().replace(regex, "<a class='term-link inactive-link' data-value='$1' href='#'>$1</a>"));

		//				setTimeout(function () { $(container).find("a").switchClass("inactive-link", "active-link", "slow"); }, 0);
		//			});
		//		},

		LoadBooks: function (callback)
		{
			getBooks(function (books)
			{
				callback(books);
			});
		}

	};

}).call(this);