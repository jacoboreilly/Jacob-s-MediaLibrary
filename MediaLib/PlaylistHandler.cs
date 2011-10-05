using System;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;
using MediaLib.Model;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace MediaLib
{
    public class PlaylistHandler : IHttpHandler
    {
        private readonly static XmlSerializer serializer = new XmlSerializer(typeof(playlist));

        /// <summary>
        /// You will need to configure this handler in the web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;

            Response.ContentType = "text/xml";// "application/xspf+xml";

            // expect the form
            // ~/folder_playlist.xml
            var path = HttpUtility.UrlDecode( Request.AppRelativeCurrentExecutionFilePath.Substring(2, Request.AppRelativeCurrentExecutionFilePath.Length - 15));

            var mediaRootWeb = ConfigurationManager.AppSettings["mediaPath"];
            var mediaRootPath = Request.MapPath(mediaRootWeb);
            var mediaPath = Path.Combine(mediaRootPath, path);

            // Get album art
            var mediaPathDir = new DirectoryInfo(mediaPath);
            var artFiles = from de in mediaPathDir.GetFiles()
                           where de.Name.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase) ||
                                de.Name.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase)
                           orderby de.Length descending
                           select de.FullName;
            string albumArtFileName = artFiles.FirstOrDefault();
            if (albumArtFileName != null)
            {
                if ((File.GetAttributes(albumArtFileName) & FileAttributes.Hidden) > 0)
                {
                    File.SetAttributes(albumArtFileName, FileAttributes.Normal);
                }
                albumArtFileName = GetUrlFromMediaPath(Request, mediaRootWeb, mediaRootPath, albumArtFileName);
            }

            var qry = from d in Directory.GetFiles(mediaPath)
                      let dname = Path.GetFileName(d)
                      where d.EndsWith(".mp3", StringComparison.InvariantCultureIgnoreCase)
                      select new track()
                      {
                          location = GetUrlFromMediaPath(Request, mediaRootWeb, mediaRootPath, d),
                          creator = "creator",
                          title = dname,
                          annotation = "annotation",
                          image = albumArtFileName,
                          info = "info"
                      };
            var pl = new playlist()
            {
                trackList = qry.ToList()
            };

            serializer.Serialize(Response.OutputStream, pl);
        }

        #endregion

        private static string GetUrlFromMediaPath(HttpRequest request, string mediaRootWeb, string mediaRootPath, string path)
        {
            return request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped) + mediaRootWeb + "/" + path.Substring(mediaRootPath.Length + 1).Replace("\\", "/");
        }
    }
}
