<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<MediaLib.Models.LibraryFolder>" ContentType="text/xml" %>
<playlist version="1">
    <tracklist>
    <%
        if (Model != null)
        {
            foreach (var track in Model.Media)
            {
            %>
            <track>
                <location><%= System.Security.SecurityElement.Escape(track.ResourceUrl)%></location>
                <title><%= System.Security.SecurityElement.Escape(track.Name)%></title>
                <image><%= System.Security.SecurityElement.Escape(track.ImageUrl)%></image>
            </track>
            <%
            }
        }
    %>
    </tracklist>
</playlist>