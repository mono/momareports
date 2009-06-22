<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Moma.Web.Helpers.MomaNode>" %>
<%@ Import Namespace="Moma.Web.Helpers" %>
<%@ Import Namespace="Moma.Web.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Moma - API Tree
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
	$().ready (function() {
		$("#members_tv").treeview ({
		collapsed: true,
		url: "tree"
		}); });
</script>
<h2>API Tree</h2>
<div>These are all the APIs with issues across all MoMA reports except:
<ul>
<li>The entire System.Management namespace</li>
<li>The entire System.Management.Instrumentation namespace</li>
<li>The System.Web.Configuration.BrowserCapabilitiesFactory class</li>
<li>The entire System.Web.UI.WebControls.WebParts namespace</li>
</ul>
</div>
<br />
<%= String.Format (Model.HtmlFormat, "<b>" + HttpUtility.HtmlEncode (Model.Text) + "</b>", Model.TotalMissing, Model.TotalTodo, Model.TotalNiex) %>
<ul id="members_tv">
</ul>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="CssContent" runat="server">
    <link rel="stylesheet" type="text/css" href="<%= Url.Content ("~/Content/redmond/jquery-ui-1.7.1.custom.css") %>" />
    <link rel="stylesheet" type="text/css" href="<%= Url.Content ("~/Content/jquery.treeview.css")%>"/>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ScriptContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content ("~/Scripts/jquery-1.3.2.min.js") %>"></script>
    <script type="text/javascript" src="<%= Url.Content ("~/Scripts/jquery.treeview.min.js") %>"></script>
    <script type="text/javascript" src="<%= Url.Content ("~/Scripts/jquery.treeview.async.js") %>"></script>
    <script type="text/javascript" src="<%= Url.Content ("~/Scripts/jquery-ui-1.7.1.custom.min.js") %>"></script>
</asp:Content>
