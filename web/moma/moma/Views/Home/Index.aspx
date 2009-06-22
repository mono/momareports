<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Moma.Web.Models.MomaDataSet>" %>
<%@ Import Namespace="Moma.Web.Models" %>
<%@ Import Namespace="Moma.Web.Helpers" %>
<%@ Import Namespace="System.Drawing" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Moma Reports - Home
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
	<table style="border: none 0px;" cellpadding="0" cellspacing="0">
	<tr>
		<td width="30%" align="left" valign="top" style="border: none 0;">
	<h3>Missing APIs</h3>
	<%= Html.ActionLink ("[ By Application ]", "byapp-missing", "summary") %>
	<%= Html.ActionLink ("[ By Use Count ]", "byuse-missing", "summary") %>

	<h3>Todo APIs</h3>
	<%= Html.ActionLink ("[ By Application ]", "byapp-todo", "summary") %>
	<%= Html.ActionLink ("[ By Use Count ]", "byuse-todo", "summary") %>

	<h3>Not Implemented APIs</h3>
	<%= Html.ActionLink ("[ By Application ]", "byapp-niex", "summary") %>
	<%= Html.ActionLink ("[ By Use Count ]", "byuse-niex", "summary") %>

	<h3>P/Invoke</h3>
	<%= Html.ActionLink ("[ By Application ]", "byapp-pinvoke", "summary") %>
	<%= Html.ActionLink ("[ By Use Count ]", "byuse-pinvoke", "summary") %>
		</td>
		<td width="60%" style="border: none 0;">
	<h3 style="text-align:center;">Reports by Number of Issues</h3>
	<%
		ChartViewData chart = new ChartViewData ();
		chart.Colors = "00FF00,CCFF00,FFDD00,FFBB00,FF7700,FF0000";
		chart.Title = "All Issues (TODO, Not Implemented, Missing, P/Invoke)";
		chart.Size = new Size (600, 200);
		chart.NameColumn = "Title";
		chart.DataColumn = "Apps";
		chart.Rows = Model.ByIssue.Rows;
		Html.RenderPartial ("Chart", chart);
	%>
	<br style="margin-bottom: 35px;"/>
	<%	
		chart = new ChartViewData ();
		chart.Colors = "00FF00,CCFF00,FFDD00,FFBB00,FF7700,FF0000";
		chart.Title = "Excluding TODO";
		chart.Size = new Size (600, 200);
		chart.NameColumn = "Title";
		chart.DataColumn = "Apps";
		chart.Rows = Model.ByIssueNoTodo.Rows;
		Html.RenderPartial ("Chart", chart);
	%>
	<br style="margin-bottom: 35px;"/>
	<%	
		chart = new ChartViewData ();
		chart.Colors = "00FF00,CCFF00,FFDD00,FFBB00,FF7700,FF0000";
		chart.Title = "Excluding P/Invoke";
		chart.Size = new Size (600, 200);
		chart.NameColumn = "Title";
		chart.DataColumn = "Apps";
		chart.Rows = Model.ByIssueNoPInvoke.Rows;
		Html.RenderPartial ("Chart", chart);
	%>
		</td>
	</tr>
	</table>
</asp:Content>
