/// <reference path="app.js" />
/// <reference path="filehelper.js" />
/// <reference path="../libs/Class.js" />

(function ()
{

	var _firstLoad = true, _browsePath = "", _editRights = false, _currentPath = "";

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
				content.EditRights = _editRights;
				_currentPath = path;
				$("#tmpFolderContent").tmpl(content).appendTo($("#uxContents").empty());
				$(window).resize();

				App.SetTitle(path || "Home Directory");
				if (Modernizr.history && push) history.pushState({ Path: path }, path, App.ResolveUrl("~/Files/Browse/" + path));
			},
			error: function (xhr)
			{
				App.HandleError(xhr);
			}
		});
	}

	var domSetup = function (me)
	{
		window.onload = function ()
		{
			_firstLoad = true;

			if (_browsePath != "")
			{
				if (Modernizr.history) history.replaceState({ Path: _browsePath }, _browsePath, App.ResolveUrl("~/Files/Browse/" + _browsePath));
			}

			loadContents(_browsePath, false);
			setTimeout(function () { _firstLoad = false; }, 0);
		};

		if (Modernizr.history)
		{
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


		$(document)
		.on("click", ".folder-expand", function ()
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
		})
		.on("click", ".folder-view", function ()
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
		})
		.on("click", "#btnUpload", function ()
		{
			var div = $(this).parents("h1").find(".subcontent");
			
			if (div.is(":visible")) div.slideUp('fast');
			else div.slideDown('fast');

			return false;
		})
		.on("click", "#btnNewFolder", function ()
		{
			var name = prompt("Enter the name of the new folder you'd like to add to this directory:");

			if ($.trim(name).length < 1) return false;

			$.ajax(
			{
				url: App.ResolveUrl("~/Ajax/Files/NewFolder"),
				type: "POST",
				data: {
					name: name,
					path: "/" + _currentPath
				},
				success: function (data)
				{
					App.ShowAlert("Directory Added", "success");
					loadFolders(_currentPath, function (data)
					{
						if (_currentPath == "")
						{
							$("#tmpFolders").tmpl(data).appendTo($("#ulFolders").empty());
						}
						else
						{
							var a = $("[data-path='" + _currentPath + "']");
							a.siblings(".folder-expand").removeClass("ui-icon-plus").addClass("ui-icon-minus");
							$("#tmpFolders").tmpl(data).appendTo($("<ul class='folders'></ul>").appendTo(a.parent("li")));
						}
					});

				},
				error: function (xhr)
				{
					App.HandleError(xhr);
				}
			});

			return false;
		})
		.on("change", "[type='file']", function ()
		{
			$(this).parent("form").submit();
		});

	};

	this.App.Files = Class.extend(
	{
		init: function (browsePath, editRights)
		{
			_browsePath = browsePath;
			_editRights = editRights;

			loadFolders("", function (data)
			{
				$("#tmpFolders").tmpl(data).appendTo("#ulFolders");
			});

			App.FileHelper.ReadyPopover();

			domSetup(this);
		}

	});

}).call(this);