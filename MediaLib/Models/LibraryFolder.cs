using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MediaLib.Models
{
    public class LibraryFolder
    {
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Link { get; set; }
        public virtual LibraryFolder Parent { get; set; }
        public virtual string ImageUrl { get; set; }
        public virtual List<LibraryFolder> Children { get; set; }
        public virtual List<MediaItem> Media { get; set; }
    }
}