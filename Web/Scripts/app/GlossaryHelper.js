/// <reference path="app.js" />
/// <reference path="../libs/json2.js" />
/// <reference path="../libs/consolelog.js" />

(function ()
{
	var _terms;

	var getTerms = function (callback)
	{
		if (_terms != null)
		{
			callback(_terms);
			return;
		}

		if (Modernizr.localstorage)
		{

			var data = localStorage.getItem("terms");
			if (data != null)
			{
				data = JSON.parse(data);
				var timestamp = new Date(data.Timestamp);
				var expiryDate = new Date(timestamp.setDate(timestamp.getDate() + 7)) // one week
				if (new Date() > expiryDate)
				{
					// it is too old, we need to get a new one
					localStorage.removeItem("terms")
				}
				else
				{
					_terms = data.Terms;
					callback(_terms);
					return;
				}
			}
		}

		$.ajax(
		{
			url: App.ResolveUrl("~/Ajax/Glossary/List"),
			success: function (data)
			{
				_terms = data;
				if (Modernizr.localstorage)
				{
					var obj = { "Timestamp": new Date(), "Terms": _terms }; // properties need quotes to be properly serialized
					localStorage.setItem("terms", JSON.stringify(obj));
				}

				callback(_terms);
			},
			error: function (xhr)
			{
				log(xhr)
			}
		});
	};

	this.App.GlossaryHelper = {

		HighlightTerms: function (container)
		{
			getTerms(function (terms)
			{
				var termString = $.map(terms, function (n) { return n.Term; }).join("|");

				// \\b gets the word boundaries so we only get full words
				// i gets case insensitive
				// g makes it global and not just first
				var regex = new RegExp("\\b(" + termString.replace(/(\^|\.|\*|\+|\?|\=|\!|\\|\/|\(|\)|\[|\]|\{|\})/ig, "\\$1") + ")\\b", "ig");

				$(container).html($(container).html().replace(regex, "<a class='term-link' data-value='$1' href='#'>$1</a>"));

				/*.inactive-link { color: #404040; text-decoration: none; }
				.active-link { color: #0069D6; text-decoration: underline; }*/
			});
		},

		LoadTerms: function (callback)
		{
			getTerms(function (terms)
			{
				callback(terms);
			});
		}

	};

}).call(this);