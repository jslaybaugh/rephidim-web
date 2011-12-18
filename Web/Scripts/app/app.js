/// <reference path="../libs/consolelog.js" />

(function ()
{
	var _root, _format;
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
			_root = root == "./" ? "/" : root ;
		},

		ResolveUrl: function (relative)
		{
			var resolved = relative;
			if (relative[0] == '~') resolved = _root + relative.substring(2);
			return resolved;
		},

		SetTitleFormat: function (format)
		{
			_format = format;
		},

		SetTitle: function (custompart)
		{
			document.title = _format.format(custompart);
		}

	};


}).call(this);