//-----------------------------------------------------------------------
// <copyright file="CreateRepoRequest.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Models
{
    using System.ComponentModel.DataAnnotations;

    public class CreateRepoRequest
    {
        [Required]
        public string RepoName { get; set; }

        public string Description { get; set; }
    }
}
