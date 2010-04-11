<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<TestEchoViewModel>" %>

<%@ Import Namespace="Sample.WebActors.Controllers.TestEcho" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Echo Test Form</title>

	<script language="javascript" type="text/javascript" src="../../Scripts/jquery-1.4.1.js"></script>

</head>
<body>
	<div>
		<% using (Html.BeginForm())
		 { %>
		<label for="Text">
			Enter a string to echo:</label>
		<%= Html.TextBoxFor(x => x.Text) %>
		<br />
		<textarea style="display: none" id="response" name="response" rows="10" cols="80"></textarea>
		<br />
		<input type="button" id="Send" name="Send" value="Send" />
		<div id="status">
		</div>
		<% } %>
	</div>

	<script type="text/javascript" charset="utf-8">

		function submitForm() {
			var message = { "Text": $('#Text').val() };

			$('#status').html("Sending...").show();

			$.getJSON("http://localhost:6621/Actors/Echo/Echo", message, function(json, textStatus) {
				if (textStatus == "success") {
					$('#response').html(json.Text).show();
					$('#status').html('');
				}
				else
					$('#status').html("Epic Fail: " + textStatus).show();
			}, function() { alert('fail!'); });
		}

		$(document).ready(function() {

			$("form").submit(function() {
				submitForm();
				return false;
			});

			$('#Send').click(function() {
				submitForm();
			});
		});	
	</script>

</body>
</html>
