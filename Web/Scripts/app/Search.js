/// <reference path="app.js" />
/// <reference path="../libs/Class.js" />

(function ()
{

	

	var domSetup = function (me)
	{
		$(window).resize($.throttle(250, function ()
		{
			$("#uxFiles").height($(window).height() - $("#uxHeader").outerHeight(true) - 60);
			$("#uxGlossary").height($(window).height() - $("#uxHeader").outerHeight(true) - 60);

		})).resize();


	};

	this.App.Search = Class.extend(
	{
		init: function (query)
		{
			domSetup(this);
		}

	});

}).call(this);