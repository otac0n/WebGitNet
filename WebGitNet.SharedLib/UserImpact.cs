//-----------------------------------------------------------------------
// <copyright file="UserImpact.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System;

    public class UserImpact
    {
        public string Author { get; set; }

        public int Commits { get; set; }

        public int Insertions { get; set; }

        public int Deletions { get; set; }

        public int Impact { get; set; }

        public DateTime Date { get; set; }
    }
}
