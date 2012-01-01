/// <reference path="app.js" />
/// <reference path="../libs/Class.js" />

(function ()
{

	var domSetup = function (me)
	{
		$(window).resize($.throttle(250, function ()
		{
			$("#iframeContent").height($(window).height() - 60);
			//$("#iframeContent").width($(window).width() - 100);

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