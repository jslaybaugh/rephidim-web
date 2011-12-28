﻿/// <reference path="app.js" />
/// <reference path="../libs/Class.js" />

(function ()
{

	var _path = "";

	var displayDefinition = function (push)
	{
		if (_activeTerm != null)
		{
			$("#uxList").scrollTo("[name='" + _activeTerm.Id + "']", 1000, { easing: 'swing', axis: 'y' });
			App.SetTitle(_activeTerm.Term + " - Glossary");
			if (Modernizr.history && push)
			{
				history.pushState({ TermName: _activeTerm.Term }, _activeTerm.Term, App.ResolveUrl("~/Glossary/Term/" + _activeTerm.Id));
			}

			var terms = $.map(_terms, function (n) { return n.Term; }).join("|");
			// \\b gets the word boundaries so we only get full words
			// i gets case insensitive
			// g makes it global and not just first
			var regex = new RegExp("\\b(" + terms.replace(/(\^|\.|\*|\+|\?|\=|\!|\\|\/|\(|\)|\[|\]|\{|\})/ig, "\\$1") + ")\\b", "ig");
			_activeTerm.Definition = _activeTerm.Definition.replace(regex, "<a class='term-link inactive-link' data-value='$1' href='#'>$1</a>");

			$("#tmpTermFull").tmpl(_activeTerm).appendTo($("#uxTerm").empty());
			$(window).resize();
			setTimeout(function () { $("#uxDefinition a").switchClass("inactive-link", "active-link", "slow"); }, 0);
		}
		else
		{
			App.SetTitle("Glossary");
			$("#uxTerm").html("<div class='message'><div><span>Select a term from the left to get started.</span></div></div>");
		}
	};

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

	var loadPopover = function (type, details)
	{
		var iOS = navigator.userAgent.match(/(ipad|ipod|iphone)/i);

		var parts = details.split("|");

		var data = {
			Device: iOS ? "iOS" : "Other",
			Name: parts[0],
			Extension: parts[1],
			Size: parts[2],
			Path: parts[3],
			FileDate: parts[4]
		};

		log($("#tmpPopoverTitle").tmpl(data).html())

		if (type.match(/title/i))
		{
			return $($("#tmpPopoverTitle").tmpl(data)).html();
		}
		else
		{
			return $($("#tmpPopoverContent").tmpl(data)).html();
		}
	};

	var domSetup = function (me)
	{
		//		if (Modernizr.history)
		//		{
		//			// the setTimeout is a sad workaround to make behavior consistent in chrome vs. firefox. more: http://code.google.com/p/chromium/issues/detail?id=63040
		//			setTimeout(function ()
		//			{
		//				window.onpopstate = function (event)
		//				{
		//					if (event.state != null) me.LoadContents(event.state.Path, function (data)
		//					{
		//						var content = {};
		//						content.Files = data;
		//						$("#tmpContent").tmpl(content).appendTo($("#uxContents").empty());
		//						//$("#uxContents").removeClass("hidden");

		//						//if (file == null) return false;

		//						//$("[data-file='" + file + "']").addClass("highlight");



		//					});
		//					//else getTerm(null, false);
		//				};
		//			}, 0);

		//			history.replaceState({ Path: _path }, "", App.ResolveUrl("~/Files/Path/" + _path.substring(1)));
		//		}

		$(".file-email").live("click", function (evt)
		{
			$("#uxEmailSuccess").hide();
			$("#uxEmailError").hide();
			_path = $(this).data("path");

			$(".popover").fadeOut();

			$("#uxDialog").dialog(
			{
				modal: true,
				width: 500,
				title: "Send in Email",
				resizable: false
			});
			return false;
		});

		$("#frmEmail").on("submit", function ()
		{
			$("#uxEmailSuccess").hide();
			$("#uxEmailError").hide();
			$("#uxEmailWait").show();
			$.ajax(
			{
				url: App.ResolveUrl("~/Ajax/Files/Email"),
				type: "POST",
				data: { email: $("#txtEmail").val(), path: _path },
				success: function (data)
				{
					$("#uxEmailWait").hide();
					if (data)
					{
						$("#uxEmailSuccess").fadeIn();
					}
					else
					{
						$("#uxEmailError").fadeIn();
					}
				},
				error: function (xhr)
				{
					$("#uxEmailWait").hide();
					alert(xhr.responseText);
				}
			});
			return false;
		});


		$(".file-name").live("click", function (evt)
		{
			var x = $(this);
			$(".popover").hide();

			$(this).popover(
			{

				trigger: "manual",
				html: true,
				content: function () { return loadPopover("content", x.data("details")); },
				title: function () { return loadPopover("title", x.data("details")); },
				offset: 10,
				placement: "left"
			});

			$(this).popover("show");

			return false;
		});

		$(".popover-close").live("click", function (evt)
		{
			$(this).parents(".popover").fadeOut('fast');

			return false;
		});

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

			//			if (path != null)
			//			{
			//				_Parts = path.split("/");
			//				for (i = 0; i < _Parts.length; i++)
			//				{
			//					_Parts[i] = last == "/" ? last + _Parts[i] : last + "/" + _Parts[i];
			//					last = _Parts[i];
			//				}

			//				$(".folder-view").removeClass("bold");
			//				$(".folder-view[href$='" + _Parts[_Parts.length - 1] + "']").addClass("bold");


			//				if (!_IsPostBack)
			//				{
			//					_RecurseTree(0);
			//				}

			//				$("#uxContentsTitle").text(" of " + path);

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
				content.Files = data;
				$("#tmpContent").tmpl(content).appendTo($("#uxContents").empty());
				//$("#uxContents").removeClass("hidden");

				//if (file == null) return false;

				//$("[data-file='" + file + "']").addClass("highlight");



			});
			//			}

			//			if (Modernizr.history)
			//			{
			//				history.pushState({ Path: path }, "", App.ResolveUrl("~/Files/Path/" + path));
			//			}

			return false;
		});

		$(window).resize($.throttle(250, function ()
		{
			$("#uxFolders").height($(window).height() - 60);
			$("#uxContents").height($(window).height() - 60);

		})).resize();


	};

	this.App.Files = Class.extend(
	{
		init: function (path)
		{
			_path = "/" + path;

			this.LoadFolders(_path, function (data)
			{
				$("#tmpFolders").tmpl(data).appendTo("#ulFolders");
			});

			domSetup(this);
		},

		LoadFolders: loadFolders,

		LoadContents: loadContents

	});

}).call(this);