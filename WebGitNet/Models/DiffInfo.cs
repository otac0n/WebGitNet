//-----------------------------------------------------------------------
// <copyright file="DiffInfo.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DiffInfo
    {
        private readonly IList<string> lines;

        public DiffInfo(IList<string> lines)
        {
            if (lines == null)
            {
                throw new ArgumentNullException("lines");
            }

            this.lines = lines.ToList().AsReadOnly();
        }

        public IList<string> Lines
        {
            get
            {
                return this.lines;
            }
        }
    }
}
