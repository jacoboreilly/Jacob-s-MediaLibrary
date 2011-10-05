using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MediaLib.Models;
using System.IO;
using System.Web.Routing;
using System.Diagnostics;

namespace MediaLib
{
    public class LibraryProvider
    {
        private readonly string webPath;
        private readonly string fsRootPath;
        private LibraryFolder rootLibraryFolder;
        private readonly FileSystemWatcher fsWatcher;

        public LibraryProvider(string webPath, string fsRootPath)
        {
            this.webPath = webPath;
            this.fsRootPath = fsRootPath;
            this.fsWatcher = new FileSystemWatcher(this.fsRootPath);
            fsWatcher.IncludeSubdirectories = true;
            fsWatcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName;
            fsWatcher.Changed += new FileSystemEventHandler(fsWatcher_Event);
            fsWatcher.Created += new FileSystemEventHandler(fsWatcher_Event);
            fsWatcher.Deleted += new FileSystemEventHandler(fsWatcher_Event);
            fsWatcher.Renamed += new RenamedEventHandler(fsWatcher_Event);
            fsWatcher.EnableRaisingEvents = true;
        }

        void fsWatcher_Event(object sender, FileSystemEventArgs e)
        {
            try
            {
                // See if there's a known item there
                var impactedFolder = GetByFSPath(e.FullPath);

                // If we have an item and it's a known change, we can do it here.
                if (impactedFolder != null)
                {
                    if (e.ChangeType == WatcherChangeTypes.Changed)
                    {
                        // Contents may have changed, purge
                        ((LateBoundLibraryFolder)impactedFolder).Purge();
                        // This doesn't seem to impact the parent.
                        return;
                    }
                }

                // See if this impacts a known parent
                impactedFolder = GetByFSPath(Path.GetDirectoryName(e.FullPath));
                if (impactedFolder != null)
                {
                    // A child has been modified, purge so we'll reload on next use
                    ((LateBoundLibraryFolder)impactedFolder).Purge();
                    return;
                }
            }
            catch (Exception ex)
            {
                Trace.Fail("Failed in handling FS change", ex.ToString());
            }
        }

        private LibraryFolder GetByFSPath(string path)
        {
            if (!path.StartsWith(fsRootPath, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }
            if (path.Equals(fsRootPath, StringComparison.InvariantCultureIgnoreCase))
            {
                return rootLibraryFolder;
            }
            string rootlessPath = path.Substring(fsRootPath.Length + 1);
            string webStylePath = rootlessPath.Replace('\\', '/');

            return Get(webStylePath);
        }

        public LibraryFolder Get(string path)
        {
            AssertLibraryInitialized();

            if (path == null || path == "")
            {
                return rootLibraryFolder;
            }

            var searchIn = rootLibraryFolder;
            foreach (var component in path.Split('/'))
            {
                var qry = from child in searchIn.Children
                          where child.Name.Equals(component, StringComparison.InvariantCultureIgnoreCase)
                          select child;
                var matchingChild = qry.FirstOrDefault();
                if (matchingChild == null)
                {
                    return null;
                }
                searchIn = matchingChild;
            }

            return searchIn;

            //return new LibraryFolder()
            //{
            //    Children = new List<LibraryFolder>(),
            //    ImageUrl = "",
            //    Media = new List<MediaItem>(),
            //    Name = "Test",
            //    ParentLink = ""
            //};
        }

        private void AssertLibraryInitialized()
        {
            if (rootLibraryFolder == null)
            {
                rootLibraryFolder = new LateBoundLibraryFolder(this.webPath, "", this.fsRootPath, null);
            }
        }

        class LateBoundLibraryFolder : LibraryFolder
        {
            private readonly string webRootPath;
            private readonly string webPath;
            private readonly string fileSystemPath;
            private readonly LateBoundLibraryFolder parent;

            private string name;
            private string displayName;
            private string imageUrl;
            private List<LibraryFolder> children;
            private List<MediaItem> media;

            public LateBoundLibraryFolder(string webRootPath, string webPath, string fileSystemPath, LateBoundLibraryFolder parent)
            {
                this.webRootPath = webRootPath;
                this.webPath = webPath;
                this.fileSystemPath = fileSystemPath;
                this.parent = parent;
            }

            public void Purge()
            {
                name = null;
                displayName = null;
                imageUrl = null;
                children = null;
                media = null;
            }

            public override string Name
            {
                get
                {
                    if (name == null)
                    {
                        name = parent == null ? "Media Library" : Path.GetFileName(fileSystemPath);
                    }
                    return name;
                }
                set
                {
                    // nop
                }
            }

            public override string DisplayName
            {
                get
                {
                    if (this.displayName == null)
                    {
                        displayName = SortableItemName(Name);
                    }
                    return this.displayName;
                }
                set
                {
                    // nop
                }
            }

            public override string ImageUrl
            {
                get
                {
                    if (imageUrl == null)
                    {
                        var mediaPathDir = new DirectoryInfo(fileSystemPath);
                        var artFiles = from de in mediaPathDir.GetFiles("*.jpg", SearchOption.TopDirectoryOnly)
                                       orderby de.Length descending
                                       select de.FullName;
                        string albumArtFileName = artFiles.FirstOrDefault();

                        if (albumArtFileName == null)
                        {
                            // Try deeper
                            var childItem = from child in Children
                                            where child.ImageUrl != null
                                            select child.ImageUrl;
                            var childItemImage = childItem.FirstOrDefault();
                            if (childItemImage != null)
                            {
                                return childItemImage;
                            }
                        }

                        if (albumArtFileName != null)
                        {
                            if ((File.GetAttributes(albumArtFileName) & FileAttributes.Hidden) > 0)
                            {
                                File.SetAttributes(albumArtFileName, FileAttributes.Normal);
                            }
                            albumArtFileName = JoinWebPath(webRootPath, webPath) + "/" + Path.GetFileName(albumArtFileName);
                        }
                        imageUrl = albumArtFileName;
                    }
                    return imageUrl;
                }
                set
                {
                    // nop
                }
            }

            public override string Link
            {
                get
                {
                    return RouteTable.Routes.GetVirtualPath(HttpContext.Current.Request.RequestContext,
                        "LibraryPages",
                        new RouteValueDictionary(new { path = webPath })).VirtualPath;

                }
                set
                {
                    // nop
                }
            }

            public override LibraryFolder Parent
            {
                get
                {
                    return this.parent;
                }
                set
                {
                    // nop
                }
            }

            public override List<LibraryFolder> Children
            {
                get
                {
                    if (children == null)
                    {
                        var mediaPathDir = new DirectoryInfo(fileSystemPath);
                        var subfolders = from de in mediaPathDir.GetDirectories()
                                         orderby SortableItemName(de.Name)
                                         select new LateBoundLibraryFolder(webRootPath, JoinWebPath(webPath, de.Name), fileSystemPath + @"\" + de.Name, this);
                        children = subfolders.ToList<LibraryFolder>();
                    }
                    return children;
                }
                set
                {
                    // nop
                }
            }

            public override List<MediaItem> Media
            {
                get
                {
                    if (media == null)
                    {
                        var mediaPathDir = new DirectoryInfo(fileSystemPath);
                        var mediaFiles = from de in mediaPathDir.GetFiles()
                                         where de.Extension.Equals(".mp3", StringComparison.InvariantCultureIgnoreCase)
                                         orderby de.Name
                                         select new MediaItem()
                                         {
                                             Name = Path.GetFileNameWithoutExtension(de.Name),
                                             ImageUrl = this.ImageUrl,
                                             ResourceUrl = JoinWebPath(webRootPath, webPath) + "/" + de.Name
                                         };
                        media = mediaFiles.ToList();
                    }
                    return this.media;
                }
                set
                {
                    // nop
                }
            }

            private static string JoinWebPath(string path1, string path2)
            {
                if (path1.EndsWith("/"))
                {
                    path1 = path1.Substring(0, path1.Length - 1);
                }
                if (path2.StartsWith("/"))
                {
                    return path1 + path2;
                }
                return path1 + "/" + path2;
            }

            private static string[] interestingPrefixes = { "The", "A" };

            private static string SortableItemName(string itemName)
            {
                var qry = from ip in interestingPrefixes
                          where itemName.StartsWith(ip + " ", StringComparison.InvariantCultureIgnoreCase)
                          select itemName.Substring(ip.Length + 1) + ", " + ip;
                return qry.FirstOrDefault() ?? itemName;
            }
        }
    }
}