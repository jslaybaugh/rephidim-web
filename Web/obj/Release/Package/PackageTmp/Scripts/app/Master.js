/// <reference path="app.js" />
/// <reference path="../libs/Class.js" />

(function ()
{

	var domSetup = function (me)
	{
		$(".topbar-wrapper").dropdown();
		$("#lnkViewSource").click(function ()
		{
			var w = window.open('about:blank'), s = w.document;
			s.write('<!DOCTYPE html><html><head><title>Source of ' + location.href + '</title><meta name="viewport" content="width=device-width" /></head><body></body></html>');
			s.close();

			var pre = s.body.appendChild(s.createElement("pre"));
			pre.style.overflow = 'auto';
			pre.style.whiteSpace = 'pre-wrap';
			pre.appendChild(s.createTextNode(document.documentElement.innerHTML));
		});

		$(".link-large").click(function ()
		{
			return confirm("This is a very large file and may take a while to load. The compact version is usually a better option. Continue?");
		});

		$(window).resize($.throttle(500, function ()
		{
			$(".primary-content").height($(window).height() - $(".header-content").outerHeight(true) - 60);
			setTimeout(function ()
			{
				$(".primary-content > .scrollable").each(function (i, n)
				{
					$(this).height($(this).parent(".primary-content").height() - $(this).prev("h1").outerHeight(true));
				});
			}, 100);
		})).resize();
	};

	this.App.Master = Class.extend(
	{
		init: function ()
		{
			domSetup(this);
		}

	});

}).call(this);