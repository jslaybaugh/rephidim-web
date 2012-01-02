﻿/// <reference path="app.js" />
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

	this.App.Search = Class.extend(
	{
		init: function (queryParts, termResults, fileResults)
		{
			App.SetQueryParts(queryParts);

			domSetup(this);

			$("#tmpTermSearch").tmpl({ Terms: termResults }).appendTo($("#uxGlossary").empty());
			$("#tmpFileSearch").tmpl({ Files: fileResults }).appendTo($("#uxFiles").empty());

			App.FileHelper.ReadyPopover();
		}

	});

}).call(this);