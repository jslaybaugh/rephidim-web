/// <reference path="app.js" />
/// <reference path="filehelper.js" />
/// <reference path="../libs/Class.js" />

(function ()
{

	var domSetup = function (me)
	{

	};

	this.App.Home = Class.extend(
	{
		init: function (termResults, fileResults, verseResults)
		{
			domSetup(this);

			$("#tmpVerseResults").tmpl({ Title: "Recent Verses", Verses: verseResults }).appendTo($("#uxVerses").empty());
			$("#tmpTermResults").tmpl({ Title: "Recent Terms", Terms: termResults }).appendTo($("#uxGlossary").empty());
			$("#tmpFileResults").tmpl({ Title: "Recent Files", Files: fileResults }).appendTo($("#uxFiles").empty());

			App.FileHelper.ReadyPopover();
		}

	});

}).call(this);