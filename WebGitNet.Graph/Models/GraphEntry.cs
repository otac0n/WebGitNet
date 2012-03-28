//-----------------------------------------------------------------------
// <copyright file="GraphEntry.cs" company="(none)">
//  Copyright © 2012 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Models
{
    using System;
    using System.Collections.Generic;

    public class GraphEntry
    {
        public LogEntry LogEntry { get; set; }

        public List<GitRef> Refs { get; set; }
    }
}
