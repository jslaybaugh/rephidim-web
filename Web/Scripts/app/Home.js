/// <reference path="app.js" />
/// <reference path="filehelper.js" />
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

	this.App.Home = Class.extend(
	{
		init: function (termResults, fileResults)
		{
			domSetup(this);

			$("#tmpTermRecent").tmpl({ Terms: termResults }).appendTo($("#uxGlossary").empty());
			$("#tmpFileRecent").tmpl({ Files: fileResults }).appendTo($("#uxFiles").empty());

			App.FileHelper.ReadyPopover();
		}

	});

}).call(this);