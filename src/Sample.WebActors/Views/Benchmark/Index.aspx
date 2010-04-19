<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<BenchmarkFormViewModel>" %>
<%@ Import Namespace="Sample.WebActors.Controllers"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Index</title>
</head>
<body>
	<div>
		<%
					using (var form = Html.BeginForm())
					{%>
					
					<%= Html.LabelFor(x => x.Name) %>
					<%= Html.TextBoxFor(x => x.Name) %>
					
					<%= Html.LabelFor(x => x.Address) %>
					<%= Html.TextBoxFor(x => x.Address) %>

					<%= Html.LabelFor(x => x.Age) %>
					<%= Html.TextBoxFor(x => x.Age) %>

					<%= Html.LabelFor(x => x.CreateDate) %>
					<%= Html.TextBoxFor(x => x.CreateDate) %>


					<input id="submitBtn" type="submit" value="Save" />

					
					
		<%
					}%>
	</div>
</body>
</html>
