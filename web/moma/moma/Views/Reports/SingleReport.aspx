<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Moma.Web.Models.SingleReportViewData>" %>
<%@ Import Namespace="Moma.Web.Helpers" %>
<%@ Import Namespace="Moma.Web.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Moma - Application Report
</asp:Content>

<asp:Content ID="CssContent1" ContentPlaceHolderID="CssContent" runat="server">
    <link rel="stylesheet" type="text/css" href="<%= Url.Content ("~/Content/redmond/jquery-ui-1.7.1.custom.css") %>" />
    <link rel="stylesheet" type="text/css" href="<%= Url.Content ("~/Content/jquery.treeview.css")%>"/>
</asp:Content>

<asp:Content ID="ScriptContent1" ContentPlaceHolderID="ScriptContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content ("~/Scripts/jquery-1.3.2.min.js") %>"></script>
    <script type="text/javascript" src="<%= Url.Content ("~/Scripts/jquery.treeview.min.js") %>"></script>
    <script type="text/javascript" src="<%= Url.Content ("~/Scripts/jquery-ui-1.7.1.custom.min.js") %>"></script>
    <% if (Context.Request.Url.Port != 80) { %>
    <script type="text/javascript">var disqus_developer = 1;</script>
    <% } %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
	$().ready (function() {
		$("#members_tv").treeview ({
		collapsed: true,
		animated: 100,
		control: "#masstoggler"}); });
	$(function(){
	// Tabs
	$('#tabs').tabs();
	
	//hover states on the static widgets
	$('#dialog_link, ul#icons li').hover(
		function() { $(this).addClass('ui-state-hover'); }, 
		function() { $(this).removeClass('ui-state-hover'); }
	); });
</script>
    <h2>Application Report</h2>
    <ul>
	    <li>Application ID: <%= Html.Encode (Model.App.Guid) %></li>
	    <li>Report Submitted on: <%= Html.Encode (Model.App.SubmitDate) %></li>
    <% if (!Model.App.IsDefinitionsNull ()) { %>
	    <li>Definitions File: <%= Html.Encode (Model.App.Definitions) %></li>
    <%} %>
	    <li>API Issues: <%= Model.TotalAPIs %></li>
	    <li>P/Invoke APIs: <%= Model.TotalPInvokes %></li>
    </ul>

    <br />
	<div id="tabs">
		<ul>
			<li><a href="#tabs-1">Comments</a></li>
		<% if (Model.TotalAPIs > 0) { %>
			<li><a href="#tabs-2">Issues</a></li>
		<% } %>
		<% if (Model.TotalPInvokes > 0) { %>
			<li><a href="#tabs-3">P/Invokes</a></li>
		<% } %>
		</ul>
		<div id="tabs-1">
			<div id="disqus_thread"></div>
			<noscript><a href="http://moma.disqus.com/?url=ref">View the discussion thread.</a></noscript>
			<a href="http://disqus.com" class="dsq-brlink">comments powered by <span class="logo-disqus">Disqus</span></a>
		</div>
		<div id="tabs-2">
		<% if (Model.TotalAPIs > 0) { %>
			<div>The following <%= Model.TotalAPIs %> APIs (used <%= Model.TotalUses %> times) might not work:</div>
			<br />
			<div id="masstoggler">
				<a title="Collapse entire tree" href="#"> Collapse All</a> |
				<a title="Expand entire tree" href="#"> Expand All</a>
			</div>
			<ul id="members_tv">
				<li class="open"> <%= String.Format (Model.RootNode.HtmlFormat, "<b>" +
						                  	Model.RootNode.Text + "</b>",
				                  			Model.RootNode.TotalMissing, Model.RootNode.TotalTodo,
				                  			Model.RootNode.TotalNiex) %>
				<ul>
				<% foreach (MomaNode node in Model.RootNode.ChildNodes) { %>
					<% Html.RenderPartial ("TreeNode", node); %>
				<% } %>
				</ul>
				</li>
			</ul>
		<% } else { %>
			No Issues in this report!
		<% } %>
		</div>
		<div id="tabs-3">
			<% if (Model.TotalPInvokes > 0) { %>
			<div>There are <%= Model.TotalPInvokes %> P/Invokes (used <%= Model.TotalPInvokeUses %> times):</div>
			<br />
			<table>
			<thead>
				<tr>
				<th><%= Html.Encode ("Count") %></th>
				<th><%= Html.Encode ("Library") %></th>
				<th><%= Html.Encode ("Function") %></th>
				</tr>
			</thead>
			<% foreach (MomaDataSet.PInvokesRow row in Model.PInvokes.Rows) { %>
			<tr>
				<td><%= row.Count %></td>
				<td><%= Html.Encode (row.IsLibraryNull () ? "" : row.Library) %></td>
				<td><%= Util.GetHtmlForPInvokeFunction (row.IsLibraryNull () ? "" : row.Library,
								    	row.IsFunctionNull () ? "" : row.Function)%></td>
			</tr>
			<%}%>
			</table>
			<% } %>
		</div>
	</div>
    <script type="text/javascript" src="http://disqus.com/forums/moma/embed.js"></script>
</asp:Content>
