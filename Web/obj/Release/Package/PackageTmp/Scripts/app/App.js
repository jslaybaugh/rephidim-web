/// <reference path="../libs/consolelog.js" />

(function ()
{
	var _root, _titleFormat, _icons, _queryParts, _terms, _termString;

	String.prototype.format = function ()
	{
		var s = this;
		for (var i = 0; i < arguments.length; i++)
		{
			var reg = new RegExp("\\{" + i + "\\}", "gm");
			s = s.replace(reg, arguments[i]);
		}

		return s;
	};

	this.App = {

		Regex: {
			EMAIL: /^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$/
		},

		Startup: function (root, titleFormat, icons)
		{
			_root = root;
			_titleFormat = titleFormat;
			_icons = icons;

			$("#uxAppWait").ajaxStart(function ()
			{
				$(this).show();
			});

			$("#uxAppWait").ajaxStop(function ()
			{
				$(this).hide();
			});
		},

		ResolveUrl: function (relative, scheme)
		{
			var resolved = relative;
			if (relative[0] == '~') resolved = _root + relative.substring(2);

			if (scheme != null)
			{
				return scheme + "://" + location.host + resolved;
			}
			return resolved;
		},

		SetTitle: function (custompart)
		{
			document.title = _titleFormat.format(custompart);
		},

		DateFromJson: function (jsonDate)
		{
			if (jsonDate == null) return null;
			return new Date(parseInt(jsonDate.substr(6)));
		},

		ShowAlert: function (msg, className)
		{
			$("#uxAlert").hide().removeClass("warning error success info").addClass(className).find("p").html(msg).end().fadeIn("fast");

			if (className.toUpperCase() == "SUCCESS" || className.toUpperCase() == "INFO")
			{
				_timeout = setTimeout(function () { $("#uxAlert").fadeOut("fast") }, 7000);
			}

			if (Modernizr.history)
			{
				var keys = ["SUCCESS", "ERROR", "WARNING", "INFO"];
				var oldQueries = location.search.substring(1).split("&");
				var newQueries = [];

				for (var i = 0; i < oldQueries.length; i++)
				{
					if (!(oldQueries[i].indexOf("=") >= 0 && keys.indexOf(oldQueries[i].split("=")[0].toUpperCase()) >= 0))
					{
						newQueries.push(oldQueries[i]);
					}
				}

				history.replaceState(null, null, "?" + newQueries.join("&"));
			}
		},

		ChooseIcon: function (key)
		{
			var res = _icons[key.toLowerCase()];

			if (res != null) return res;
			else return _icons["unk"];
		},

		SetQueryParts: function (queryParts)
		{
			_queryParts = queryParts;
		},

		Highlight: function (text)
		{
			if (_queryParts != null && _queryParts.length > 0)
			{
				var reg = new RegExp("(" + _queryParts.join("|") + ")", "ig");
				return text.replace(reg, "<span class='hilite'>$1</span>");
			}
			return text;
		},

		HandleError: function (xhr, el)
		{
			if (!el)
				App.ShowAlert(xhr.responseText, "error");
			else
				$(el).html(xhr.responseText).fadeIn();

			// for good measure

			log(xhr);
		}

	};


}).call(this);