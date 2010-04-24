<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Sample.WebActors.Controllers.Asynchro.AsynchroIndexViewModel>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>
		<%= Model.Title %></title>
</head>
<body>
	<div>
		Detail: <%= Model.Detail %>
		<br />
		Body: <%= Model.Body %>
	</div>
</body>
</html>
