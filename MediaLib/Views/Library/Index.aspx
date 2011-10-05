<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MediaLib.Models.LibraryFolder>" EnableViewState="False" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%: Model.Name %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="TitleContentHeader" runat="server">
	<%: Model.Name %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="crumbs">
    <% 
        var crumbStack = new Stack<MediaLib.Models.LibraryFolder>();
        var parent = Model;
        while (parent != null)
        {
            crumbStack.Push(parent);
            parent = parent.Parent;
        }
        while (crumbStack.Count > 0)
        {
            var crumb = crumbStack.Pop();
            %> 
            <a href="<%= crumb.Link %>"><%= crumb.DisplayName%></a> 
            <% 
            if (crumbStack.Count > 0)
            {
                %> / <%
            }
        }
    %>
    </div>

    <!--<img alt="Folder thumbnail" src="<%: Model.ImageUrl ?? "???" %>" />-->

    <%
        if (Model.Media.Count > 0)
        {
            %>
            <div id="mediaplayerloading" style="width:550px;height:400px;">
              <img src="<%= ResolveUrl("~/templates/default/images/busy.gif") %>" />
            </div>
            <div id="mediaplayer" class="mediashowplaylist mediashowtitle mediaplayerdialog ui-dialog ui-widget ui-widget-content ui-corner-all" style="width:550px;height:400px;">
              <div id="mediatitlebar" class="ui-widget-header ui-corner-top ui-helper-clearfix">
                <div id="mediatitlelinks">
                  <div id="mediatitlelinksinner">
                    <a href="#fullscreen" id="mediafullbutton" class="ui-state-default ui-corner-all"><span class="ui-icon ui-icon-extlink"></span></a>
                    <a href="#maximize" id="mediamaxbutton" class="ui-state-default ui-corner-all"><span class="ui-icon ui-icon-arrow-4-diag"></span></a>
                    <a href="#menu" id="mediamenubutton" class="ui-state-default ui-corner-all"><span class="ui-icon ui-icon-info"></span></a>
                  </div>
                </div>
              </div>  <div id="mediaplayer" class="ui-helper-clearfix">
                <div id="mediamenu" class="ui-tabs ui-widget ui-widget-content ui-corner-all" id="tabs">
                  <div id="mediamenuclose">
                    <span class="ui-icon ui-icon-circle-close"></span>
                  </div>
                  <ul class="ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all">
                    <li class="ui-state-default ui-corner-top ui-tabs-selected ui-state-active"><a href="#mediaembed">embed</a></li>
                    <li class="ui-state-default ui-corner-top"><a href="#mediaelink">link</a></li>
                    <li class="ui-state-default ui-corner-top"><a href="#mediainfo">info</a></li>
                  </ul>
                  <div id="mediaembed" class="menucontent ui-tabs-panel ui-widget-content ui-corner-bottom" id="mediaembed">
                    <div id="mediaembedForm" name="mediaembedForm">
                      <label for="mediaembedCode">Embed</label>
                      <input id="mediaembedCode" name="mediaembedCode" type="text" value="" readonly />
                    </div>
                  </div>
                  <div id="mediaelink" class="menucontent ui-tabs-panel ui-widget-content ui-corner-bottom" id="mediaelink">
                    <div id="mediaelinkForm" name="mediaelinkForm">
                      <label for="mediaelinkCode">URL</label>
                      <input id="mediaelinkCode" name="mediaelinkCode" type="text" value="" readonly />
                    </div>
                  </div>
                  <div id="mediainfo" class="menucontent ui-tabs-panel ui-widget-content ui-corner-bottom" id="mediainfo">
                    <p>
                      <a target="_blank" href="http://www.mediafront.org">Open Standard Media Player</a> version 0.01<br/><br/>
                      Built by <a target="_blank" href="http://www.alethia-inc.com">Alethia Inc.</a>
                    </p>
                  </div>
                </div>    <div id="medianode">
                  <div id="mediaregion">
                    <div id="mediabusy"><img src="<%= ResolveUrl("~/templates/default/images/busy.gif") %>" /></div>
                    <div id="mediaplay"><img src="<%= ResolveUrl("~/templates/default/images/play.png") %>" /></div>
                    <div id="mediapreview"></div>
                    <div id="mediadisplay"></div>
                  </div>
                </div>    <div id="mediacontrol" class="ui-widget-header ui-corner-bottom">
                  <div id="mediacontrolleft">
                    <div id="mediaplaypause" class="ui-state-default ui-corner-all">
                      <div class="on">
                        <span class="ui-icon ui-icon-play"></span>
                      </div>
                      <div class="off" style="display:none">
                        <span class="ui-icon ui-icon-pause"></span>
                      </div>
                    </div>
                    <div id="mediacurrenttime">00:00</div>
                  </div>
                  <div id="mediacontrolright">
                    <div id="mediatotaltime">00:00</div>
                    <div id="mediamute" class="ui-state-default ui-corner-all">
                      <div class="on" style="display:none">
                        <span class="ui-icon ui-icon-volume-off"></span>
                      </div>
                      <div class="off">
                        <span class="ui-icon ui-icon-volume-on"></span>
                      </div>
                    </div>
                    <div id="mediavolumebar">
                      <div id="mediavolume" class="ui-state-default">
                        <div id="mediavolumeupdate">
                          <div id="mediavolumehandle">
                            <span class="ui-icon ui-icon-triangle-1-s"></span>
                          </div>
                          <div class="ui-state-active"></div>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div id="mediacontrolcenter">
                    <div id="mediaseekbar">
                      <div id="mediaseek" class="ui-state-default">
                        <div id="mediaseekupdate">
                          <div id="mediaseekhandle">
                            <span class="ui-icon ui-icon-triangle-1-s"></span>
                          </div>
                          <div class="ui-state-active"></div>
                        </div>
                        <div id="mediaseekprogress"><div class="ui-state-hover"></div></div>
                      </div>
                    </div>
                  </div>
                </div>  </div>
              <div id="mediaplaylist">
                <div id="mediascrollwrapper">
                  <div id="mediabusy" class="ui-state-default ui-state-disabled"><img src="<%= ResolveUrl("~/templates/default/images/busy.gif") %>" /></div>
                  <div id="mediascroll" class="ui-helper-clearfix">
                    <div id="mediascrollbarwrapper">
                      <div id="mediascrollup" class="ui-state-default"><span class="ui-icon ui-icon-triangle-1-n"></span></div>
                      <div id="mediascrollbar">
                        <div id="mediascrolltrack" class="ui-widget-content">
                          <div id="mediascrollhandle" class="ui-state-default"><span class="ui-icon ui-icon-grip-dotted-horizontal"></span></div>
                        </div>
                      </div>
                      <div id="mediascrolldown" class="ui-state-default"><span class="ui-icon ui-icon-triangle-1-s"></span></div>
                    </div>         <div id="medialistmask">
                      <div id="medialist">
                        <div id="mediateaser" class="ui-state-default">
                          <div id="mediaimage" class="mediafield ui-widget-content" type="image" field="thumbnail"></div>
                          <div id="mediatitle" class="mediafield ui-helper-clearfix" type="text" field="title">Sample Title</div>
                        </div>        </div>
                    </div>
                  </div>
                </div>
                <div id="mediapager" class="ui-widget-header ui-corner-bottom">
                  <div id="mediapagerleft">
                    <div id="mediaprevpage" class="mediapagerlink ui-state-default ui-corner-all"><span class="ui-icon ui-icon-seek-prev"></span></div>
                    <div id="medialoadprev" class="mediapagerlink ui-state-default ui-corner-all"><span class="ui-icon ui-icon-triangle-1-w"></span></div>
                    <div id="mediaprev" class="mediapagerlink ui-state-default ui-corner-all"><span class="ui-icon ui-icon-carat-1-w"></span></div>
                  </div>
                  <div id="mediapagerright">
                    <div id="medianext" class="mediapagerlink ui-state-default ui-corner-all"><span class="ui-icon ui-icon-carat-1-e"></span></div>
                    <div id="medialoadnext" class="mediapagerlink ui-state-default ui-corner-all"><span class="ui-icon ui-icon-triangle-1-e"></span></div>
                    <div id="medianextpage" class="mediapagerlink ui-state-default ui-corner-all"><span class="ui-icon ui-icon-seek-next"></span></div>
                  </div>
                </div></div>
            </div>
            <script type="text/javascript">$(document).ready(function() { $(".mediaplayerdialog").mediaplayer({ playlist:"<%= Model.Link %>.xml" }); }); </script>
            <%
        }
    %>

    <!--<div class="container">-->
    <% 
        char firstLetter = ' ';
        foreach (var subfolder in Model.Children)
        {
            char thisFirstLetter = char.ToUpper(subfolder.DisplayName[0]);
            if (thisFirstLetter != firstLetter)
            {
                if (firstLetter != ' ')
                {
                    %>
                    <div class="spacer">&nbsp;</div>
                    </div>
                    <%
                }
                %>
                <p class="sectiontitle"><%= thisFirstLetter %></p>
                <div class="container">
                <div class="spacer">&nbsp;</div>
                <%
                firstLetter = thisFirstLetter;
            }
            %>
            <div class="float">
                <p>
                    <a href="<%= subfolder.Link %>">
                        <img src="<%= subfolder.ImageUrl ?? ResolveUrl("~/Content/noart2.png") %>" alt="Album art" width="128" height="128" /><br /><%= subfolder.DisplayName %>
                    </a>
                </p>
            </div>
        <% } %>
        <div class="spacer">&nbsp;</div>
    </div>

    <ul id="tracklisting">
        <% foreach (var mediaItem in Model.Media)
           { %>
           <li><a href="<%= mediaItem.ResourceUrl %>"><%= mediaItem.Name %></a></li>
        <% } %>
    </ul>

</asp:Content>
