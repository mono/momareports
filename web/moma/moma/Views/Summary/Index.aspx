<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Mono Migration Analyzer Reports - Browse APIs
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Missing APIs</h2>
    <%= Html.ActionLink ("[ By Application ]", "byapp-missing", "summary") %>
    <%= Html.ActionLink ("[ By Use Count ]", "byuse-missing") %>
    
    <h2>Todo APIs</h2>
    <%= Html.ActionLink ("[ By Application ]", "byapp-todo", "summary") %>
    <%= Html.ActionLink ("[ By Use Count ]", "byuse-todo") %>
    
    <h2>Not Implemented APIs</h2>
    <%= Html.ActionLink ("[ By Application ]", "byapp-niex", "summary") %>
    <%= Html.ActionLink ("[ By Use Count ]", "byuse-niex") %>

    <h2>P/Invoke</h2>
    <%= Html.ActionLink ("[ By Application ]", "byapp-pinvoke", "summary") %>
    <%= Html.ActionLink ("[ By Use Count ]", "byuse-pinvoke") %>
</asp:Content>
