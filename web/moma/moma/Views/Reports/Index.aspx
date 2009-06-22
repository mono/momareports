<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Moma.Web.Models.ReportsViewData>" %>
<%@ Import Namespace="Moma.Web.Helpers" %>
<%@ Import Namespace="Moma.Web.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Moma - Reports List
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Reports List</h2>
	<div id="table_data">
		<br />
		<table>
		<thead>
		<tr>
			<td colspan="7">
				<div style="float:left">
				<%= Model.Pager.CurrentFirst %> to <%= Model.Pager.CurrentLast %> of <%= Model.Pager.TotalCount %>.
				</div>
				<div style="float:right"><% Html.RenderPartial ("ViewPager", Model.Pager); %></div>
			</td>
		</tr>
		<tr>
			<th>Submission Date</th>
			<th>Definitions</th>
			<th>Detail Link</th>
			<th>TODOs</th>
			<th>Missing</th>
			<th>Not Implemented</th>
			<th>P/Invoke</th>
		</tr>
		</thead>
		<% foreach (MomaDataSet.ApplicationsRow row in Model.Data.Applications.Rows) { %>
		<tr>
			<td><%= Html.Encode (row.SubmitDate) %></td>
			<td><%= row.IsDefinitionsNull () ? "" : Html.Encode (row.Definitions) %></td>
			<td><%= Html.RouteLink ("Full Report", "Single Report", new { guid = row.Guid })%></td>
			<td class="tar"><%= row.TotalTodo %></td>
			<td class="tar"><%= row.TotalMissing %></td>
			<td class="tar"><%= row.TotalNiex %></td>
			<td class="tar"><%= row.TotalPInvoke %></td>
		</tr>
		<%}%>
		<tr>
			<td colspan="7">
				<div align="right"><% Html.RenderPartial ("ViewPager", Model.Pager); %></div>
			</td>
		</tr>
		</table>
	</div>
</asp:Content>
