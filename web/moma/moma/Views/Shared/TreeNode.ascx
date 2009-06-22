<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Moma.Web.Helpers.MomaNode>" %>
<%@ Import Namespace="Moma.Web.Helpers" %>
<li>
<% if (!Model.HasChildren) { %>
	<%= Html.RouteLink (Model.Text, "Apis", new { apiname = Util.GetUrlFromApiName (Model.FullName)}) %>
	<%= String.Format ("Used {0} time{1} ", Model.NumberOfUses, Model.NumberOfUses != 1 ? "s" : "") %>
<% } %>
<%= String.Format (Model.HtmlFormat, "<b>" + Html.Encode (Model.Text) + "</b>", Model.TotalMissing, Model.TotalTodo, Model.TotalNiex) %>
<% if (Model.HasChildren) { %>
	<ul>
	<% foreach (MomaNode node in Model.ChildNodes) { %>
		<% Html.RenderPartial ("TreeNode", node); %>
	<% } %>
	</ul>
<% } %>
</li>

