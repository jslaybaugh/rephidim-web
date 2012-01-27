/// <reference path="app.js" />
/// <reference path="filehelper.js" />
/// <reference path="../libs/Class.js" />

(function ()
{

	var _firstLoad = true, _browsePath = "";

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
				App.HandleError(xhr);
			}
		});
	}

	var loadContents = function (path, push)
	{
		$.ajax(
		{
			url: App.ResolveUrl("~/Ajax/Files/Contents"),
			data: { path: path },
			success: function (data)
			{
				var content = {};
				content.Path = path;
				content.Files = data;
				$("#tmpFolderContent").tmpl(content).appendTo($("#uxContents").empty());
				$(window).resize();

				if (push) history.pushState({ Path: path }, path, App.ResolveUrl("~/Files/Browse/" + path));
			},
			error: function (xhr)
			{
				App.HandleError(xhr);
			}
		});
	}

	var domSetup = function (me)
	{
		if (Modernizr.history)
		{
			window.onload = function ()
			{
				_firstLoad = true;

				if (_browsePath != "")
				{
					history.replaceState({ Path: _browsePath }, _browsePath, App.ResolveUrl("~/Files/Browse/" + _browsePath));
				}
				
				loadContents(_browsePath, false);
				setTimeout(function () { _firstLoad = false; }, 0);
			};

			window.onpopstate = function (event)
			{
				if (_firstLoad)
				{
					_firstLoad = false;
				}
				else
				{
					if (event.state != null)
					{
						loadContents(event.state.Path, false);
					}
					else
					{
						loadContents("", false);
					}
				}

			};
		}


		$(".folder-expand").live("click", function ()
		{
			$(".popover").fadeOut();
			var a = $(this);

			if (a.hasClass("ui-icon-plus"))
			{
				loadFolders(a.data("path"), function (data)
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
				loadFolders(path, function (data)
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

			loadContents(path, true);

			return false;
		});

	};

	this.App.Files = Class.extend(
	{
		init: function (browsePath)
		{
			_browsePath = browsePath;
			
			loadFolders("", function (data)
			{
				$("#tmpFolders").tmpl(data).appendTo("#ulFolders");
			});

			App.FileHelper.ReadyPopover();

			domSetup(this);
		}

	});

}).call(this);