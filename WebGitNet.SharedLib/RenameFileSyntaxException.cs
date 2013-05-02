//-----------------------------------------------------------------------
// <copyright file="RenameFileSyntaxException.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class RenameFileSyntaxException : Exception
    {
        public RenameFileSyntaxException(string message)
            : base(message)
        {
        }
    }
}
