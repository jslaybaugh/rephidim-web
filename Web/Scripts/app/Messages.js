/// <reference path="app.js" />
/// <reference path="../libs/Class.js" />

(function ()
{
	var saveMessage = function ()
	{
		$("#uxMessageDialogError").hide();
		$.ajax(
		{
			url: App.ResolveUrl("~/Ajax/Messages/Save"),
			type: "POST",
			data: {
				id: $("#hdnMessageId").val(),
				value: $("#txtMessageValue").val(),
				style: $("input[name=style]:checked").val(),
				isActive: $("#chkMessageIsActive").is(":checked"),
				onHomePage: $("#chkMessageOnHomePage").is(":checked"),
				onLoginPage: $("#chkMessageOnLoginPage").is(":checked")
			},
			success: function (data)
			{
				location = App.ResolveUrl("~/Admin/Messages?success=Message+Saved!");
			},
			error: function (xhr)
			{
				App.HandleError(xhr, "#uxMessageDialogError");
			}
		});
	};

	var editMessage = function (data)
	{
		$("#tmpMessageEdit").tmpl(data).appendTo($("#uxMessageDialog").empty());
		$("#uxMessageDialog").dialog(
		{
			width: 800,
			height: 500,
			title: "Add/Edit Message",
			modal: true,
			resizable: false,
			buttons: [
				{
					text: "Cancel",
					click: function () { $(this).dialog("close"); }
				},
				{
					text: "Save",
					click: saveMessage
				}
			]
		});
	};

	var domSetup = function (me)
	{
		$(".delete-btn").click(function ()
		{
			if (!confirm("Are you sure you want to permanently delete this message?\n\nYou can always just inactivate it if you want to keep it for later.")) return false;
			var btn = $(this);
			var id = btn.data("id");

			$.ajax(
			{
				url: App.ResolveUrl("~/Ajax/Messages/Delete"),
				type: "POST",
				data: { id: id },
				success: function (data)
				{
					location = location;
				},
				error: function (xhr)
				{
					App.HandleError(xhr);
				}
			});

			return false;
		});

		$(".edit-btn").click(function ()
		{
			var btn = $(this);
			var id = btn.data("id");

			$.ajax(
			{
				url: App.ResolveUrl("~/Ajax/Messages/Edit"),
				data: { id: id },
				success: editMessage,
				error: function (xhr)
				{
					App.HandleError(xhr);
				}
			});

			return false;
		});

		$(".add-btn").click(function ()
		{
			editMessage({ Id: 0, Value: "", Style: "info", IsActive: true, OnHomePage: true, OnLoginPage: true });

			return false;
		});
	};

	this.App.Messages = Class.extend(
	{
		init: function ()
		{
			domSetup(this);
		}

	});

}).call(this);