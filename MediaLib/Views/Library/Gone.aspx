<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MediaLib.Models.LibraryFolder>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Gone!
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Gone!</h2>

    Unfortunately, we cannot find the media you are looking for.  To look for media that we can find, try the <a href="<%= ResolveUrl("~/Library") %>"><%= MediaLib.MvcApplication.LibraryProvider.Get("").DisplayName %></a>.

</asp:Content>
