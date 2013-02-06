/// <reference path="app.js" />
/// <reference path="filehelper.js" />
/// <reference path="../libs/Class.js" />

(function ()
{

	var domSetup = function (me)
	{


	};

	this.App.Search = Class.extend(
	{
		init: function (queryParts, termResults, fileResults, verseResults)
		{
			App.SetQueryParts(queryParts);

			domSetup(this);

			$("#tmpVerseResults").tmpl({ Title: "Verse Results", Verses: verseResults }).appendTo($("#uxVerses").empty());
			$("#tmpTermResults").tmpl({ Title: "Glossary Results", Terms: termResults }).appendTo($("#uxGlossary").empty());
			$("#tmpFileResults").tmpl({ Title: "File Results", Files: fileResults }).appendTo($("#uxFiles").empty());

			App.FileHelper.ReadyPopover();
		}

	});

}).call(this);