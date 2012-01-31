/// <reference path="app.js" />
/// <reference path="../libs/Class.js" />

(function ()
{

	var domSetup = function (me)
	{
		$(".topbar-wrapper").dropdown();
		$("#lnkViewSource").click(function ()
		{
			var w = window.open('about:blank'), s = w.document;
			s.write('<!DOCTYPE html><html><head><title>Source of ' + location.href + '</title><meta name="viewport" content="width=device-width" /></head><body></body></html>');
			s.close();

			var pre = s.body.appendChild(s.createElement("pre"));
			pre.style.overflow = 'auto';
			pre.style.whiteSpace = 'pre-wrap';
			pre.appendChild(s.createTextNode(document.documentElement.innerHTML));
		});

		$(".link-large").live("click", function ()
		{
			alert("This is a very large file and may take a while to load. If you encounter issues, please try the compact version.");
		});

		$(".help").click(function ()
		{
			$("#uxHelpDialog").dialog(
				{
					width: 800,
					height: 500,
					title: "System Help / Report an Issue",
					modal: true,
					resizable: false,
					open: function ()
					{
						$(".help-tabs").tabs();
						$("#txtHelpEmail").val(_LastUsedEmail);
						$("#txtHelpUrl").val(location.href);
					}
				});
		});

		$("#frmHelp").live("submit", function ()
		{
			var email = $("#txtHelpEmail").val();
			var type = $("#cmbHelpType").val();
			var issue = $("#txtHelpIssue").val();
			var url = $("#txtHelpUrl").val();

			if ($.trim(email) == "" || !email.match(App.Regex.EMAIL) || $.trim(issue) == "" || $.trim(url) == "")
			{
				App.ShowAlert("One or more fields are empty or invalid.", "error");
				return false;
			}

			$.ajax(
				{
					url: App.ResolveUrl("~/Ajax/Emails/SubmitIssue"),
					data: { email: email, type: type, content: issue, url: url },
					type: "POST",
					success: function (data)
					{
						$("#uxHelpDialog").dialog("close");
						App.ShowAlert("Issue submitted! Thank you!", "success")
					},
					error: function (xhr)
					{
						App.HandleError(xhr);
					}
				});

			return false;
		});

		$(window).resize($.throttle(500, function ()
		{
			$(".primary-content").height($(window).height() - $(".header-content").outerHeight(true) - 60);
			setTimeout(function ()
			{
				$(".primary-content > .scrollable").each(function (i, n)
				{
					$(this).height($(this).parent(".primary-content").height() - $(this).prev("h1").outerHeight(true));
				});
			}, 100);
		})).resize();
	};

	this.App.Master = Class.extend(
	{
		init: function ()
		{
			domSetup(this);
		}

	});

}).call(this);