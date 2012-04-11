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
    using System.Linq;
    using System.Text;

    public class NodeInfo
    {
        public string Hash { get; set; }

        public int Color { get; set; }

        public override string ToString()
        {
            return this.Hash + ":" + this.Color;
        }
    }
}
