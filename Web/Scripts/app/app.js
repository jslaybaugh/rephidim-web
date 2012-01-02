/// <reference path="../libs/consolelog.js" />

(function ()
{
	var _root, _titleFormat, _icons, _queryParts;
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
			$("#uxAlert").hide().removeClass("warning error success info").addClass(className).find("p").html(msg).end().fadeIn();
		},

		ChooseIcon: function (key)
		{
			return _icons[key.toLowerCase()];
		},

		SetQueryParts: function (queryParts)
		{
			_queryParts = queryParts;
		},

		Highlight: function (text)
		{
			var reg = new RegExp("(" + _queryParts.join("|") + ")", "ig");
			return text.replace(reg, "<span class='hilite'>$1</span>");
		}

	};


}).call(this);