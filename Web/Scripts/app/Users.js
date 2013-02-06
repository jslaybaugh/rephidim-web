/// <reference path="app.js" />
/// <reference path="../libs/Class.js" />

(function ()
{
	var saveUser = function ()
	{
		$("#uxUserDialogError").hide();
		$.ajax(
		{
			url: App.ResolveUrl("~/Ajax/Users/Save"),
			type: "POST",
			data: {
				id: $("#hdnUserId").val(),
				name: $("#txtUserName").val(),
				password: $("#txtPassword").val(),
				rights: $("#txtRights").val(),
				isActive: $("#chkUserIsActive").is(":checked"),
			},
			success: function (data)
			{
				location = App.ResolveUrl("~/Admin/Users?success=User+Saved!");
			},
			error: function (xhr)
			{
				App.HandleError(xhr, "#uxUserDialogError");
			}
		});
	};

	var editUser = function (data)
	{
		$("#tmpUserEdit").tmpl(data).appendTo($("#uxUserDialog").empty());
		$("#uxUserDialog").dialog(
		{
			width: 800,
			height: 500,
			title: "Add/Edit User",
			modal: true,
			resizable: false,
			buttons: [
				{
					text: "Save",
					click: saveUser,
					"class": "btn btn-primary"
				},
				{
					text: "Cancel",
					click: function () { $(this).dialog("close"); },
					"class": "btn"
				}
			]
		});
	};

	var domSetup = function (me)
	{
		$(".edit-btn").click(function ()
		{
			var btn = $(this);
			var id = btn.data("id");

			$.ajax(
			{
				url: App.ResolveUrl("~/Ajax/Users/Edit"),
				data: { id: id },
				success: editUser,
				error: function (xhr)
				{
					App.HandleError(xhr);
				}
			});

			return false;
		});

		$(".add-btn").click(function ()
		{
			editUser({ Id: 0, Name: "", Rights: "", Password: "", IsActive: true });

			return false;
		});
	};

	this.App.Users = Class.extend(
	{
		init: function ()
		{
			domSetup(this);
		}

	});

}).call(this);