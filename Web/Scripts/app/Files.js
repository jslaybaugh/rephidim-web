/// <reference path="app.js" />
/// <reference path="../libs/Class.js" />

(function ()
{

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

		$(document)
		.on("click", ".folder-expand", function ()
		{
			var a = $(this);

			if (a.find(".icon-plus").length > 0)
			{
				a.find(".icon-plus").removeClass().addClass("icon-minus");
				a.parent("li").load(App.ResolveUrl("~/Files/Folders"), a.data("path"));
			}
			else
			{
				a.parent("li").find("ul").remove();
				a.find(".icon-minus").removeClass().addClass("icon-plus");
			}

			return false;
		})
		.on("click", ".folder-view", function ()
		{
			var a = $(this);
			var path = a.data("path");

			if (a.siblings(".folder-expand").find(".icon-plus").length > 0)
			{
				a.siblings(".folder-expand").find(".icon-plus").removeClass().addClass("icon-minus");
				a.parent("li").load(App.ResolveUrl("~/Files/Folders"), a.data("path"));
			}
			else
			{
				a.parent("li").find("ul").remove();
				a.siblings(".folder-expand").find(".icon-minus").removeClass().addClass("icon-plus");
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
		init: function (browsePath)
		{
			_browsePath = browsePath;

			$("#uxFolders .scrollable").load(App.ResolveUrl("~/Files/Folders"), { path: "" });

			domSetup(this);
		}

	});

}).call(this);