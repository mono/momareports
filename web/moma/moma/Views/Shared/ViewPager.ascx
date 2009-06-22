<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Moma.Web.Helpers.Pager>" %>
<div class="pager">
	<% if (Model.HasPreviousPage) {%>
		<%= Html.RouteLink ("\u25c4", new { page = Model.PageIndex - 1 })%>
	<%} else {%>
		<%= "\u25c4" %>
	<%} %>
	&nbsp;&nbsp;
	<%= Model.PageIndex %> / <%= Model.TotalPages %>
	&nbsp;&nbsp;
	<% if (Model.HasNextPage) {%>
		<%= Html.RouteLink ("\u25ba", new { page = Model.PageIndex + 1}) %>
	<%} else {%>
		<%= "\u25ba" %>
	<%} %>
</div>

