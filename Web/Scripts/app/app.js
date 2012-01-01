/// <reference path="../libs/consolelog.js" />

(function ()
{
	var _root, _format, _icons;
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


		ShowPopup: function (relativeUrl, windowName)
		{
			return window.open(this.ResolveUrl(relativeUrl), windowName, 'status=yes,toolbars=no,scrollbars=yes,resizable=yes,width=900,height=800').focus(); void 0;
		},

		SetRoot: function (root)
		{
			_root = root;

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

		SetTitleFormat: function (format)
		{
			_format = format;
		},

		SetTitle: function (custompart)
		{
			document.title = _format.format(custompart);
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

		SetIconTable: function (data)
		{
			_icons = data;
		},

		ChooseIcon: function (key)
		{
			return _icons[key.toLowerCase()];
		}

	};


}).call(this);