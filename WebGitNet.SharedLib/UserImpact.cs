//-----------------------------------------------------------------------
// <copyright file="UserImpact.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
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

        public DateTime Date { get; set; }

        public int Deletions { get; set; }

        public int Impact { get; set; }

        public int Insertions { get; set; }
    }
}
