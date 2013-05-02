//-----------------------------------------------------------------------
// <copyright file="AccessLevel.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Models
{
    using System;

    [Flags]
    public enum AccessLevel
    {
        None = 0,
        Execute = 1,
        Write = 2,
        Read = 4,
    }
}
