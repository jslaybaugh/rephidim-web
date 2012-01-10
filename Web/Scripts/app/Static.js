/// <reference path="app.js" />
/// <reference path="../libs/Class.js" />

(function ()
{

	var domSetup = function (me)
	{
		$(window).resize($.throttle(250, function ()
		{
			$("#uxContent").height($(window).height() - 60);
		})).resize();
	};

	this.App.Static = Class.extend(
	{
		init: function ()
		{
			domSetup(this);
		}

	});

}).call(this);