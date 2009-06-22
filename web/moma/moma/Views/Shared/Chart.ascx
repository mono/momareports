<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Moma.Web.Models.ChartViewData>" %>
<%@ Import Namespace="Moma.Web.Models" %>
<%@ Import Namespace="Moma.Web.Helpers" %>
<%= String.Format ("<img alt='chart' width='{3}' height='{4}' src='{0}&chtt={1}&chco={2}' />",
	GChart.GetURLFromData (Model.Size, Model.Rows, Model.NameColumn, Model.DataColumn),
	Model.Title,
	Model.Colors,
	Model.Size.Width, Model.Size.Height) %>

