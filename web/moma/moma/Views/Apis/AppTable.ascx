<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Moma.Web.Models.ApiViewData>" %>
<script type="text/javascript" src="<%= Url.Content ("../../Scripts/jquery.dataTables.js") %>"></script>
<script type="text/javascript">
/*$().ready(function() {
	$('#example').dataTable( {
		"bProcessing" : true,
		"iDisplayLength" : 25,
		"bPaginate" : true,
		"bSort" : true
	});
} );*/
</script>
<% if (Model.Data.Applications.Count > 0) { %>
    <table id="example">
	<thead>
        <tr>
            <th>
                Report Detail
            </th>
            <th>
                Submission Date
            </th>
            <th>
                Missing
            </th>
            <th>
                TODO
            </th>
            <th>
                Not Implemented
            </th>
            <th>
                P/Invoke
            </th>
        </tr>
        </thead>
        <tbody>
    <% foreach (var app in Model.Data.Applications) { %>
        <tr>
		<td><%= Html.RouteLink ("Full Report", "Single Report", new { guid = app.Guid }) %></td>
		<td><%= Html.Encode (app.SubmitDate) %></td>
		<td class="tar"><%= Html.Encode (app.TotalMissing) %></td>
		<td class="tar"><%= Html.Encode (app.TotalTodo) %></td>
		<td class="tar"><%= Html.Encode (app.TotalNiex) %></td>
		<td class="tar"><%= Html.Encode (app.TotalPInvoke) %></td>
        </tr>
    <% } %>
        </tbody>
    </table>
<% } %>
