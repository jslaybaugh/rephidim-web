/// <reference path="app.js" />
/// <reference path="../libs/Class.js" />

(function ()
{
	var domSetup = function (me)
	{
		$(".delete-btn").click(function ()
		{
			if (!confirm("Are you sure you want to permanently delete this email?")) return false;
			var btn = $(this);
			var id = btn.data("id");

			$.ajax(
			{
				url: App.ResolveUrl("~/Ajax/Emails/Delete"),
				type: "POST",
				data: { email: id },
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

		$("#frmEmail").submit(function ()
		{
			var email = $("#txtEmail").val();

			if ($.trim(email) == "" || !email.match(/^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$/))
			{
				App.ShowAlert("A valid email is required.", "error");
				return false;
			}

			$.ajax(
			{
				url: App.ResolveUrl("~/Ajax/Emails/Save"),
				data: { email: email },
				type: "POST",
				success: function (data)
				{
					location = App.ResolveUrl("~/Admin/Emails?success=Email+Added!");
				},
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
		});
	};

	this.App.Emails = Class.extend(
	{
		init: function ()
		{
			domSetup(this);
		}

	});

}).call(this);