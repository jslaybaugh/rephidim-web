/// <reference path="app.js" />
/// <reference path="filehelper.js" />
/// <reference path="../libs/Class.js" />

(function ()
{

	var _path = "";

	var loadFolders = function (path, callback)
	{
		$.ajax(
		{
			url: App.ResolveUrl("~/Ajax/Files/Folders"),
			data: { path: path },
			success: function (data)
			{
				if (callback) callback(data);
			},
			error: function (xhr)
			{
				alert(xhr.statusText);
			}
		});
	}

	var loadContents = function (path, callback)
	{
		$.ajax(
		{
			url: App.ResolveUrl("~/Ajax/Files/Contents"),
			data: { path: path },
			success: function (data)
			{
				if (callback) callback(data);
			},
			error: function (xhr)
			{
				alert(xhr.statusText);
			}
		});
	}

	var domSetup = function (me)
	{
		$(".folder-expand").live("click", function ()
		{
			$(".popover").fadeOut();
			var a = $(this);

			if (a.hasClass("ui-icon-plus"))
			{
				me.LoadFolders(a.data("path"), function (data)
				{
					a.removeClass("ui-icon-plus").addClass("ui-icon-minus");
					$("#tmpFolders").tmpl(data).appendTo($("<ul class='folders'></ul>").appendTo(a.parent("li")));
				});
			}
			else
			{
				a.parent("li").find("ul").remove();
				a.removeClass("ui-icon-minus").addClass("ui-icon-plus");
			}

			return false;
		});

		$(".folder-view").live("click", function ()
		{
			$(".popover").fadeOut();
			var a = $(this);

			var path = a.data("path");

			if (a.siblings(".folder-expand").hasClass("ui-icon-plus"))
			{
				me.LoadFolders(path, function (data)
				{
					a.siblings(".folder-expand").removeClass("ui-icon-plus").addClass("ui-icon-minus");
					$("#tmpFolders").tmpl(data).appendTo($("<ul class='folders'></ul>").appendTo(a.parent("li")));
				});
			}
			else
			{
				a.parent("li").find("ul").remove();
				a.siblings(".folder-expand").removeClass("ui-icon-minus").addClass("ui-icon-plus");
			}


			me.LoadContents(path, function (data)
			{
				var content = {};
				content.Path = path;
				content.Files = data;
				$("#tmpContent").tmpl(content).appendTo($("#uxContents").empty());
				$(window).resize();

			});

			return false;
		});

		$(window).resize($.throttle(250, function ()
		{
			$("#uxFolders").height($(window).height() - 60);
			$("#uxContents").height($(window).height() - 60);
			$("#uxFiles").height($(window).height() - $("#uxPath").outerHeight(true) - 60);

		})).resize();


	};

	this.App.Files = Class.extend(
	{
		init: function (path)
		{
			this.LoadFolders(_path, function (data)
			{
				$("#tmpFolders").tmpl(data).appendTo("#ulFolders");
			});

			App.FileHelper.ReadyPopover();

			domSetup(this);
		},

		LoadFolders: loadFolders,

		LoadContents: loadContents

	});

}).call(this);