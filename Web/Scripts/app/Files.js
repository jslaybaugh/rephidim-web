/// <reference path="app.js" />
/// <reference path="../libs/Class.js" />

(function ()
{
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
			url: "/Files/AjaxFolders",
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
			url: "/Files/AjaxContents",
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
		//		if (Modernizr.history)
		//		{
		//			// the setTimeout is a sad workaround to make behavior consistent in chrome vs. firefox. more: http://code.google.com/p/chromium/issues/detail?id=63040
		//			setTimeout(function()
		//			{
		//				window.onpopstate = function(event)
		//				{
		//					if (event.state != null) getTerm(event.state.TermName, false);
		//					else getTerm(null, false);
		//				};
		//			}, 0);
		//		}

		$(".folder-expand").live("click", function ()
		{
			var a = $(this);

			if (a.hasClass("ui-icon-plus"))
			{
				a.removeClass("ui-icon-plus").addClass("ui-icon-minus");
				me.LoadFolders(a.data("path"), function (data)
				{
					$("#tmpFolders").tmpl(data).appendTo($("<ul class='folders'></ul>").appendTo(a.parent("li")));
				});
			}
			else
			{
				a.removeClass("ui-icon-minus").addClass("ui-icon-plus");
				a.parent("li").find("ul").remove();
			}

			return false;
		});

		$(".folder-view").live("click", function ()
		{
			log('here');
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

			return false;
		});

		$(window).resize($.throttle(250, function ()
		{
			$("#uxFolders").height($(window).height() - 60);
			$("#uxContents").height($(window).height() - 60);
			//$("#uxDefinition").height($(window).height() - $("#uxTermName").outerHeight(true) - 60);
		})).resize();

	};

	this.App.Files = Class.extend(
	{
		init: function ()
		{
			this.LoadFolders("/", function (data)
			{
				$("#tmpFolders").tmpl(data).appendTo("#ulFolders");
			});

			domSetup(this);
		},

		LoadFolders: loadFolders,

		LoadContents: loadContents

	});

}).call(this);