/// <reference path="app.js" />

(function ()
{
	var _path = "";

	var loadPopover = function (type, details)
	{
		var iOS = navigator.userAgent.match(/(ipad|ipod|iphone)/i);

		var parts = details.split("|");

		_path = parts[3];

		var data = {
			Device: iOS ? "iOS" : "Other",
			Name: parts[0],
			Extension: parts[1],
			Size: parts[2],
			Path: parts[3],
			FileDate: parts[4]
		};


		if (type.match(/title/i))
		{
			return $($("#tmpPopoverTitle").tmpl(data)).html();
		}
		else
		{
			return $($("#tmpPopoverContent").tmpl(data)).html();
		}
	};

	this.App.FileHelper = {

		ReadyPopover: function ()
		{
			$(document)
			.on("submit", "#frmEmail", function ()
			{
				var email = $("#txtEmail").val();

				if ($.trim(email) == "" || !email.match(App.Regex.EMAIL))
				{
					App.ShowAlert("A valid email is required.", "error");
					return false;
				}

				var origText = $("#btnEmail").val();

				$("#btnEmail").attr("disabled", true).val("Please Wait");
				$.ajax(
				{
					url: App.ResolveUrl("~/Ajax/Files/Email"),
					type: "POST",
					data: { email: $("#txtEmail").val(), path: _path },
					success: function (data)
					{
						$("#btnEmail").attr("disabled", false).val(origText);
						if (data)
						{
							App.ShowAlert("Email sent to " + $("#txtEmail").val() + "!", "success");
						}
						else
						{
							App.ShowAlert("Error sending email! Please refresh and try again!", "error");
						}
					},
					error: function (xhr)
					{
						$("#btnEmail").attr("disabled", false).val(origText);
						App.ShowAlert(xhr.responseText, "error");
					}
				});

				return false;
			})
			.on("click", ".file-name", function (evt)
			{
				var x = $(this);
				$(".popover").hide();

				$(this).popover(
				{

					trigger: "manual",
					html: true,
					title: function () { return loadPopover("title", x.data("details")); },
					content: function () { return loadPopover("content", x.data("details")); },
					offset: 10,
					placement: "left"
				});

				$(this).popover("show");
				$(".email-box").val(window._LastUsedEmail);

				return false;
			})
			.on("click", ".popover-close", function (evt)
			{
				$(this).parents(".popover").fadeOut('fast');

				return false;
			});
		}

	};

}).call(this);