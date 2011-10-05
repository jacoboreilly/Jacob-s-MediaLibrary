using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MediaLib.Models;

namespace MediaLib.Controllers
{
    public class LibraryController : Controller
    {
        //
        // GET: /Library/*

        public ActionResult Index(string path)
        {
            var libFolder = MvcApplication.LibraryProvider.Get(path);

            // Do they want the playlist xml for this path instead?
            if (libFolder == null && path.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
            {
                // Try to get a folder using the same path minus the .xml part
                var pathMinusTheDotXml = path.Substring(0, path.Length - 4);
                libFolder = MvcApplication.LibraryProvider.Get(pathMinusTheDotXml);
                return View("Playlist", libFolder);
            }

            if (libFolder == null)
            {
                return View("Gone");
            }

            return View(libFolder);
        }

    }
}
