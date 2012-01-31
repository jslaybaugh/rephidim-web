/// <reference path="app.js" />
/// <reference path="GlossaryHelper.js" />
/// <reference path="../libs/Class.js" />
/// <reference path="../libs/jquery.ba-throttle-debounce.min.js" />

(function ()
{
	var _activeTerm, _firstLoad = true, _editRights = false;

	var displayDefinition = function (push)
	{
		var data = {};
		data.ActiveTerm = _activeTerm;
		data.EditRights = _editRights;

		if (_activeTerm == null)
		{
			App.SetTitle("Glossary");
			$("#tmpTermFull").tmpl(data).appendTo($("#uxTerm").empty());
			return;
		}

		if ($("[name='" + _activeTerm.Id + "']").length > 0)
			setTimeout(function () { $("#uxList .scrollable").scrollTo("[name='" + _activeTerm.Id + "']", 1000, { easing: 'swing', axis: 'y' }); }, 1000);

		App.SetTitle(_activeTerm.Term);

		if (Modernizr.history && push)
		{
			history.pushState({ TermName: _activeTerm.Term }, _activeTerm.Term, App.ResolveUrl("~/Glossary/Term/" + _activeTerm.Id));
		}

		$("#tmpTermFull").tmpl(data).appendTo($("#uxTerm").empty());
		$(window).resize();

		App.GlossaryHelper.HighlightTerms("#uxDefinition");
		App.ScriptureHelper.HighlightVerses("#uxDefinition");


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
			resizable: false,
			buttons: [
				{
					text: "Cancel",
					click: function () { $(this).dialog("close"); }
				},
				{
					text: "Save",
					click: saveTerm
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
		window.onload = function ()
		{
			_firstLoad = true;

			if (_activeTerm != null)
			{
				if (Modernizr.history) history.replaceState({ TermName: _activeTerm.Term }, _activeTerm.Term, App.ResolveUrl("~/Glossary/Term/" + _activeTerm.Id));
			}
			displayDefinition(false);
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
				resizable: false,
				buttons: [
					{
						text: "Cancel",
						click: function () { $(this).dialog("close"); }
					},
					{
						text: "Save",
						click: updateVersion
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
				resizable: false,
				buttons: [
					{
						text: "Cancel",
						click: function () { $(this).dialog("close"); }
					},
					{
						text: "DELETE",
						click: function () { deleteTerm(id); }
					}
				]
			});
			return false;
		});

		$(".term-add").live("click", function ()
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

	};

	this.App.Glossary = Class.extend(
	{
		init: function (activeTerm, version, editRights)
		{
			_activeTerm = activeTerm;
			_editRights = editRights;

			var list = { EditRights: editRights, Version: version };
			App.GlossaryHelper.LoadTerms(function (terms)
			{
				list.Terms = $.map(terms, function (n)
				{
					if (n.IsNew == null) n.IsNew = false;
					if (n.IsModified == null) n.IsModified = false;

					return n;
				});

				$("#tmpTermList").tmpl(list).appendTo($("#uxList").empty());
			});

			domSetup(this);
		}

	});

}).call(this);