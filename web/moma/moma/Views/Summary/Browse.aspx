<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SummaryViewData>" %>
<%@ Import Namespace="Moma.Web.Helpers" %>
<%@ Import Namespace="Moma.Web.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= Model.Title %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<h2><%= Html.Encode (Model.Title) %></h2>
	<div id="table_data" style="overflow:auto">
		<br />
		<table>
		<thead>
		<tr>
			<td colspan="10">
				<div style="float:left;"><% Html.RenderPartial ("ViewPager", Model.Pager); %></div>
				<div style="margin-left:10em;">
				[<%= Model.Pager.CurrentFirst %> to <%= Model.Pager.CurrentLast %> of <%= Model.Pager.TotalCount %>]
				</div>
			</td>
		</tr>
		<tr>
		<% foreach (int i in Model.Columns) { %>
			<th><%= Html.Encode (Model.Data.Report.Columns [i].Caption) %></th>
		<%}%>
		</tr>
		</thead>
		<% foreach (MomaDataSet.ReportRow row in Model.Data.Report.Rows) { %>
		<tr>
			<% foreach (int i in Model.Columns) { %>
				<% if (i != Model.ApiLinkColumn) { %>
					<% if (row [i] is int) { %>
						<td class="tar"><%= Html.Encode (row [i])%></td>
					<% } else { %>
						<td><%= Html.Encode (row [i])%></td>
					<% } %>
				<% } else { %>
					<td><%= Html.RouteLink (row [i].ToString (), "Apis", new { apiname = Util.GetUrlFromApiName (row [i].ToString ())}) %></td>
				<% } %>
			<%}%>
		</tr>
		<%}%>
		<tr>
			<td colspan="10">
				<div style="float:left;"><% Html.RenderPartial ("ViewPager", Model.Pager); %></div>
			</td>
		</tr>
		</table>
	</div>
</asp:Content>
