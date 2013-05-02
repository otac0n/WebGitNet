//-----------------------------------------------------------------------
// <copyright file="RepoSettings.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Models
{
    public class RepoSettings
    {
        public string Description { get; set; }

        public bool IsArchived { get; set; }
    }
}