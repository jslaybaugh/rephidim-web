/// <reference path="app.js" />
/// <reference path="../libs/Class.js" />
/// <reference path="../libs/jquery.ba-throttle-debounce.min.js" />

(function ()
{
	var _terms, _activeTerm;

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

	var getTerm = function (termName, push)
	{
		if (termName != null)
		{
			$.ajax(
			{
				url: App.ResolveUrl("~/Ajax/Glossary/Details"),
				data: { term: termName },
				success: function (data)
				{
					_activeTerm = data;
					displayDefinition(push);
				},
				error: function (xhr)
				{
					log(xhr);
				}
			});
		}
		else
		{
			_activeTerm = null;
			displayDefinition(false);
		}
	};

	var domSetup = function (me)
	{
		if (Modernizr.history)
		{
			// the setTimeout is a sad workaround to make behavior consistent in chrome vs. firefox. more: http://code.google.com/p/chromium/issues/detail?id=63040
			setTimeout(function ()
			{
				window.onpopstate = function (event)
				{
					if (event.state != null) getTerm(event.state.TermName, false);
					else getTerm(null, false);
				};
			}, 100);
		}

		$(".term-link").live("click", function ()
		{
			getTerm($(this).data("value"), true);
			return false;
		});

		$(window).resize($.throttle(250, function ()
		{
			$("#uxList").height($(window).height() - $("#uxFooter").outerHeight(true) - 60);
			$("#uxTerm").height($(window).height() - 60);
			$("#uxDefinition").height($(window).height() - $("#uxTermName").outerHeight(true) - 60);
		})).resize();

		$("#tmpTermList").tmpl(_terms).appendTo("#ulTerms");

		if (_activeTerm != null && Modernizr.history)
		{
			history.replaceState({ TermName: _activeTerm.Term }, _activeTerm.Term, App.ResolveUrl("~/Glossary/Term/" + _activeTerm.Id));
		}
		displayDefinition(false);

	};

	this.App.Glossary = Class.extend(
	{
		init: function (terms, activeTerm)
		{
			_terms = terms;
			_activeTerm = activeTerm;

			domSetup(this);
		}

	});

}).call(this);