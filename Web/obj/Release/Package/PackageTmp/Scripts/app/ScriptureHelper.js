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
				data = JSON.parse(data);
				var timestamp = new Date(data.Timestamp);
				var expiryDate = new Date(timestamp.setDate(timestamp.getDate() + 7)) // one week
				if (new Date() > expiryDate)
				{
					// it is too old, we need to get a new one
					localStorage.removeItem("books")
				}
				else
				{
					_books = data.Books;
					callback(_books);
					return;
				}
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
					var obj = { "Timestamp": new Date(), "Books": _books }; // properties need quotes to be properly serialized
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

		HighlightVerses: function (container)
		{
			getBooks(function (books)
			{
				var booklist = $.makeArray($.map(books, function (n) { return n.Name + (n.Aliases != "" ? "|" + n.Aliases : ""); })).join("|");
				var regex = new RegExp("\\b(" + booklist + ")[.,]?[ ]*(\\d{1,3})[:;]?(\\d{1,3})?", "ig")
				
				$(container).html($(container).html().replace(regex, "<a class='verse-link' href='" + App.ResolveUrl("~/Scripture/$1/$2/$3") + "'>$1 $2:$3</a>").replace(/:</ig, "<"));
			});
		},

		LoadBooks: function (callback)
		{
			getBooks(function (books)
			{
				callback(books);
			});
		}

	};

}).call(this);