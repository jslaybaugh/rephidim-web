/// <reference path="app.js" />
/// <reference path="../libs/Class.js" />
/// <reference path="../libs/jquery.ba-throttle-debounce.min.js" />

(function ()
{
	var _terms, _activeTerm, _firstLoad = true, _editRights = false;

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

		_activeTerm.EditRights = _editRights;

		if ($("[name='" + _activeTerm.Id + "']").length > 0)
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
				App.HandleError(xhr);
			}
		});
	};

	var saveTerm = function ()
	{
		$("#uxTermsDialogError").hide();
		$.ajax(
		{
			url: App.ResolveUrl("~/Ajax/Glossary/Save"),
			type: "POST",
			data: {
				id: $("#hdnTermEditId").val(),
				term: $("#txtTermEditTerm").val(),
				definition: $("#txtTermEditDefinition").val(),
				updateDate: $("#chkTermEditModify").is(":checked")
			},
			success: function (data)
			{
				if ($("#hdnTermEditId").val() == "0")
				{
					location = App.ResolveUrl("~/Glossary/Term/" + data.Id + "?success=Term+Added!");
				}
				else
				{
					_activeTerm = data;
					displayDefinition(false);
					App.ShowAlert("Term " + data.Term + " Saved!", "success");
					$("#uxTermsDialog").dialog("close");
				}
			},
			error: function (xhr)
			{
				App.HandleError(xhr, "#uxTermsDialogError");
			}
		});
	};

	var editTerm = function (data)
	{
		if (data == null)
		{
			data = { Id: 0, Term: "", Definition: "" };
		}
		$("#tmpTermEdit").tmpl(data).appendTo($("#uxTermsDialog").empty());
		$("#uxTermsDialog").dialog(
		{
			width: 800,
			height: 500,
			title: data.Term == "" ? "New Term" : data.Term,
			modal: true,
			buttons: [
				{
					text: "Save",
					click: saveTerm
				},
				{
					text: "Cancel",
					click: function () { $(this).dialog("close"); }
				}
			]
		});
	};

	var deleteTerm = function (id)
	{
		if (!confirm("Are you REALLY sure?!")) return false;
		$.ajax(
		{
			url: App.ResolveUrl("~/Ajax/Glossary/Delete"),
			type: "POST",
			data: {
				id: id
			},
			success: function (data)
			{
				location = App.ResolveUrl("~/Glossary?success=Term+Deleted!");
			},
			error: function (xhr)
			{
				App.HandleError(xhr);
			}
		});
	};

	var updateVersion = function ()
	{
		$.ajax(
		{
			url: App.ResolveUrl("~/Ajax/Glossary/UpdateVersion"),
			type: "POST",
			data: {
				version: $("#txtVersionEdit").val()
			},
			success: function (data)
			{
				$("#btnVersion").text("v." + $("#txtVersionEdit").val());
				$("#uxTermsDialog").dialog("close");
				App.ShowAlert("Version updated!", "success");
			},
			error: function (xhr)
			{
				App.HandleError(xhr);
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

		$("#btnVersion").live("click", function ()
		{
			var ver = $(this).data("id");
			var data = { Version: ver };
			$("#tmpTermVersion").tmpl(data).appendTo($("#uxTermsDialog").empty());
			$("#uxTermsDialog").dialog(
			{
				width: 500,
				height: 200,
				title: "Update Version",
				modal: true,
				buttons: [
					{
						text: "Save",
						click: updateVersion
					},
					{
						text: "Cancel",
						click: function () { $(this).dialog("close"); }
					}
				]
			});
			return false;
		});

		$("#btnDelete").live("click", function ()
		{
			var id = $(this).data("id");
			$("#uxTermsDialog").html("Are you sure you want to PERMANENTLY delete this term???<br/><br/>This action CANNOT be undone!");
			$("#uxTermsDialog").dialog(
			{
				width: 500,
				height: 200,
				title: "DELETE?!?!?!?!?!",
				modal: true,
				buttons: [
					{
						text: "DELETE",
						click: function () { deleteTerm(id); }
					},
					{
						text: "Cancel",
						click: function () { $(this).dialog("close"); }
					}
				]
			});
			return false;
		});

		$("#btnAdd").live("click", function ()
		{
			editTerm();
			return false;
		});

		$("#btnEdit").live("click", function ()
		{
			$.ajax(
			{
				url: App.ResolveUrl("~/Ajax/Glossary/Edit"),
				data: { id: $(this).data("id") },
				success: editTerm,
				error: function (xhr)
				{
					App.HandleError(xhr);
				}
			});

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
		init: function (terms, activeTerm, editRights)
		{
			_terms = $.map(terms, function (n)
			{
				if (n.IsNew == null) n.IsNew = false;
				if (n.IsModified == null) n.IsModified = false;

				return n;
			});

			_activeTerm = activeTerm;
			_editRights = editRights;

			domSetup(this);
		}

	});

}).call(this);