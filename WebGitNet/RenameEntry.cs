using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGitNet
{
    public enum RenameField
    {
        None = 0,
        Name,
        Email,
    }

    public enum RenameStyle
    {
        Exact,
        CaseInsensitive,
        Regex,
    }

    public class RenameEntry
    {
        public RenameStyle RenameStyle { get; set; }

        public RenameField SourceField { get; set; }

        public string Match { get; set; }

        public Destination[] Destinations { get; set; }

        public class Destination
        {
            public RenameField Field { get; set; }

            public string Replacement { get; set; }
        }
    }
}