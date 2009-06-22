<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Moma.Web.Models.ApiViewData>" %>
<%@ Import Namespace="Moma.Web.Helpers" %>
<%@ Import Namespace="Moma.Web.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	API - <%= Model.Title %>
</asp:Content>

<asp:Content ID="CssContent1" ContentPlaceHolderID="CssContent" runat="server">
    <link rel="stylesheet" type="text/css" href="<%= Url.Content ("~/Content/redmond/jquery-ui-1.7.1.custom.css") %>" />
</asp:Content>

<asp:Content ID="ScriptContent1" ContentPlaceHolderID="ScriptContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content ("~/Scripts/jquery-1.3.2.min.js") %>"></script>
    <script type="text/javascript" src="<%= Url.Content ("~/Scripts/jquery-ui-1.7.1.custom.min.js") %>"></script>
    <% if (Context.Request.Url.Port != 80) { %>
    <script type="text/javascript">var disqus_developer = 1;</script>
    <% } %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
	$(function(){
	// Tabs
	$('#tabs').tabs();
	
	//hover states on the static widgets
	$('#dialog_link, ul#icons li').hover(
		function() { $(this).addClass('ui-state-hover'); }, 
		function() { $(this).removeClass('ui-state-hover'); }
	); });
</script>
    <h2><%= Html.Encode (Model.Title) %></h2>
    <ul>
    <li>Status: <%= Html.Encode (Model.Status) %></li>
    <% if (Model.Comment != null) { %>
	    <li>Comment: <%= Html.Encode(Model.Comment) %></li>
    <% } %>
    <% if (!Model.Data.Members [0].IsFixed) { %>
	    <li>Apps using it: <%= Model.Data.ApiUse[0].TotalApps %></li>
	    <li>Number of calls: <%= Model.Data.ApiUse[0].TotalCalls %></li>
    <% } %>
    </ul>
	<div id="tabs">
		<ul>
			<li><a href="#tabs-1">Comments</a></li>
		<% if (Model.Data.Applications.Count > 0) { %>
			<li><a href="#tabs-2">Applications</a></li>
		<% } %>
		</ul>
		<div id="tabs-1">
			<div id="disqus_thread"></div>
			<noscript><a href="http://moma.disqus.com/?url=ref">View the discussion thread.</a></noscript>
			<a href="http://disqus.com" class="dsq-brlink">comments powered by <span class="logo-disqus">Disqus</span></a>
		</div>
		<div id="tabs-2">
			<% Html.RenderPartial ("AppTable", Model); %>
		</div>
	</div>
	<script type="text/javascript" src="http://disqus.com/forums/moma/embed.js"></script>
</asp:Content>
