/// <reference path="app.js" />
/// <reference path="../libs/Class.js" />
/// <reference path="../libs/jquery.ba-throttle-debounce.min.js" />

(function ()
{
	var _terms, _activeTerm, _firstLoad = true, _popFired = false;

	var pad = function (currentLength)
	{
		var totalNeeded = 2000;
		var s = " ";
		for (var i = 0; i < totalNeeded - currentLength; i++)
		{
			s += "&zwnj;";
		}

		return s;
	};

	var displayDefinition = function (push)
	{
		if (_activeTerm == null)
		{
			App.SetTitle("Glossary");
			$("#uxTerm").html("<div class='message'><div><span>Select a term from the left to get started.</span></div></div>");
			return;
		}

		$("#uxList .scrollable").scrollTo("[name='" + _activeTerm.Id + "']", 1000, { easing: 'swing', axis: 'y' });
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
		_activeTerm.Definition = _activeTerm.Definition.replace(regex, "<a class='term-link inactive-link' data-value='$1' href='#'>$1</a>")// + pad(0);

		$("#tmpTermFull").tmpl(_activeTerm).appendTo($("#uxTerm").empty());
		$(window).resize();
		setTimeout(function () { $("#uxDefinition a").switchClass("inactive-link", "active-link", "slow"); }, 0);

	};

	var getTerm = function (termName, push)
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
	};

	var domSetup = function (me)
	{
		if (Modernizr.history)
		{
			window.onload = function ()
			{
				_firstLoad = true;

				if (_activeTerm != null)
				{
					history.replaceState({ TermName: _activeTerm.Term }, _activeTerm.Term, App.ResolveUrl("~/Glossary/Term/" + _activeTerm.Id));
				}
				displayDefinition(false);
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
					if (event.state != null) getTerm(event.state.TermName, false);
					else
					{
						_activeTerm = null;
						displayDefinition(false);
					}
				}

			};
		}

		$(".term-link").live("click", function ()
		{
			getTerm($(this).data("value"), true);
			return false;
		});

		$(window).resize($.throttle(250, function ()
		{
			$("#uxList").height($(window).height() - 60);
			$("#uxTerm").height($(window).height() - 60);
		})).resize();

		$("#tmpTermList").tmpl(_terms).appendTo("#ulTerms");

	};

	this.App.Glossary = Class.extend(
	{
		init: function (terms, activeTerm)
		{
			_terms = $.map(terms, function (n)
			{
				if (n.IsNew == null) n.IsNew = false;
				if (n.IsModified == null) n.IsModified = false;

				return n;
			});
			_activeTerm = activeTerm;

			domSetup(this);
		}

	});

}).call(this);