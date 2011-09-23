//-----------------------------------------------------------------------
// <copyright file="RepoImpact.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Models
{
    using System.Linq;

    public class RepoImpact
    {
        public IOrderedEnumerable<UserImpact> AllTime { get; set; }

        public IOrderedEnumerable<ImpactWeek> Weekly { get; set; }
    }
}