//-----------------------------------------------------------------------
// <copyright file="GraphEntry.cs" company="(none)">
//  Copyright © 2012 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Models
{
    public class NodeInfo
    {
        public int Color { get; set; }

        public string Hash { get; set; }

        public override string ToString()
        {
            return this.Hash + ":" + this.Color;
        }
    }
}
