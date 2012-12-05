/// <reference path="app.js" />
/// <reference path="GlossaryHelper.js" />
/// <reference path="../libs/Class.js" />
/// <reference path="../libs/jquery.ba-throttle-debounce.min.js" />

(function ()
{
	var finalizePage = function (id, term)
	{

		if (id != null)
		{
			if ($("#uxList .scrollable [name='" + id + "']").length > 0)
			{
				setTimeout(function () { $("#uxList .scrollable").scrollTo("[name='" + id + "']", 1000, { easing: 'swing', axis: 'y' }); }, 1000);
			}
		}
		else if (term != null)
		{
			if ($("#uxList .scrollable [data-term='" + term.toUpperCase() + "']").length > 0)
			{
				setTimeout(function () { $("#uxList .scrollable").scrollTo("[data-term='" + term.toUpperCase() + "']", 1000, { easing: 'swing', axis: 'y' }); }, 1000);
			}
		}

		App.GlossaryHelper.HighlightTerms("#uxDefinition");
		App.ScriptureHelper.HighlightVerses("#uxDefinition");
		$(window).resize();

	};

	//	var saveTerm = function ()
	//	{
	//		$("#uxTermsDialogError").hide();
	//		$.ajax(
	//		{
	//			url: App.ResolveUrl("~/Ajax/Glossary/Save"),
	//			type: "POST",
	//			data: {
	//				id: $("#hdnTermEditId").val(),
	//				term: $("#txtTermEditTerm").val(),
	//				definition: $("#txtTermEditDefinition").val(),
	//				updateDate: $("#chkTermEditModify").is(":checked")
	//			},
	//			success: function (data)
	//			{
	//				if ($("#hdnTermEditId").val() == "0")
	//				{
	//					location = App.ResolveUrl("~/Glossary/Term/" + data.Id + "?success=Term+Added!");
	//				}
	//				else
	//				{
	//					_activeTerm = data;
	//					displayDefinition(false);
	//					App.ShowAlert("Term " + data.Term + " Saved!", "success");
	//					$("#uxTermsDialog").dialog("close");
	//				}
	//			},
	//			error: function (xhr)
	//			{
	//				App.HandleError(xhr, "#uxTermsDialogError");
	//			}
	//		});
	//	};

	//	var editTerm = function (data)
	//	{
	//		if (data == null)
	//		{
	//			data = { Id: 0, Term: "", Definition: "" };
	//		}
	//		$("#tmpTermEdit").tmpl(data).appendTo($("#uxTermsDialog").empty());
	//		$("#uxTermsDialog").dialog(
	//		{
	//			width: 800,
	//			height: 500,
	//			title: data.Term == "" ? "New Term" : data.Term,
	//			modal: true,
	//			resizable: false,
	//			buttons: [
	//				{
	//					text: "Save",
	//					click: saveTerm,
	//					"class": "btn btn-primary"
	//				},
	//				{
	//					text: "Cancel",
	//					click: function () { $(this).dialog("close"); },
	//					"class": "btn"
	//				}
	//			]
	//		});
	//	};

	//	var deleteTerm = function (id)
	//	{
	//		if (!confirm("Are you REALLY sure?!")) return false;
	//		$.ajax(
	//		{
	//			url: App.ResolveUrl("~/Ajax/Glossary/Delete"),
	//			type: "POST",
	//			data: {
	//				id: id
	//			},
	//			success: function (data)
	//			{
	//				location = App.ResolveUrl("~/Glossary?success=Term+Deleted!");
	//			},
	//			error: function (xhr)
	//			{
	//				App.HandleError(xhr);
	//			}
	//		});
	//	};

	//	var updateVersion = function ()
	//	{
	//		$.ajax(
	//		{
	//			url: App.ResolveUrl("~/Ajax/Glossary/UpdateVersion"),
	//			type: "POST",
	//			data: {
	//				version: $("#txtVersionEdit").val()
	//			},
	//			success: function (data)
	//			{
	//				$("#btnVersion").text("v." + $("#txtVersionEdit").val());
	//				$("#uxTermsDialog").dialog("close");
	//				App.ShowAlert("Version updated!", "success");
	//			},
	//			error: function (xhr)
	//			{
	//				App.HandleError(xhr);
	//			}
	//		});
	//	};

	var domSetup = function (me)
	{
		$(window).on("statechange", function ()
		{
			var state = History.getState();
			var id = state.data.id;
			var term = state.data.term;
			$("#uxTerm").load(App.ResolveUrl("~/Glossary/Details/"), { id: id, term: term }, function ()
			{
				finalizePage(id, term);
				App.SetTitle(state.data.term);
			});
		});

		$(document)

		.on("click", ".term-link", function ()
		{
			var a = $(this);
			History.pushState({ id: a.data("id"), term: a.data("term"), url: a.attr("href") }, null, a.attr("href"));
			return false;
		})

		.on("change", "#cmbAlpha", function ()
		{
			var letter = $(this).val();
			$("#uxList .scrollable").scrollTo("[data-term^='" + letter + "']", 500, { easing: 'swing', axis: 'y' });
		})

		//		.on("click", "#btnVersion", function ()
		//		{
		//			var ver = $(this).data("id");
		//			var data = { Version: ver };
		//			$("#tmpTermVersion").tmpl(data).appendTo($("#uxTermsDialog").empty());
		//			$("#uxTermsDialog").dialog(
		//			{
		//				width: 500,
		//				height: 180,
		//				title: "Update Version",
		//				modal: true,
		//				resizable: false,
		//				buttons: [
		//					{
		//						text: "Save",
		//						click: updateVersion,
		//						"class": "btn btn-primary"
		//					},
		//					{
		//						text: "Cancel",
		//						click: function () { $(this).dialog("close"); },
		//						"class": "btn"
		//					}
		//				]
		//			});
		//			return false;
		//		})

		//		.on("click", "#btnDelete", function ()
		//		{
		//			var id = $(this).data("id");
		//			$("#uxTermsDialog").html("Are you sure you want to PERMANENTLY delete this term???<br/><br/>This action CANNOT be undone!");
		//			$("#uxTermsDialog").dialog(
		//			{
		//				width: 500,
		//				height: 200,
		//				title: "DELETE?!?!?!?!?!",
		//				modal: true,
		//				resizable: false,
		//				buttons: [
		//					{
		//						text: "DELETE",
		//						click: function () { deleteTerm(id); },
		//						"class": "btn btn-danger"
		//					},
		//					{
		//						text: "Cancel",
		//						click: function () { $(this).dialog("close"); },
		//						"class": "btn"
		//					}
		//				]
		//			});
		//			return false;
		//		})

		//		.on("click", ".term-add", function ()
		//		{
		//			editTerm();
		//			return false;
		//		})

		.on("click", "#btnEdit", function ()
		{
			var id = $(this).data("id");
			$("#uxTerm").load(App.ResolveUrl("~/Glossary/Edit/" + id));

			return false;
		});

	};

	this.App.Glossary = Class.extend(
	{
		init: function (activeTerm)
		{
			finalizePage(activeTerm.Id, activeTerm.Term);
			domSetup(this);
		}

	});

}).call(this);