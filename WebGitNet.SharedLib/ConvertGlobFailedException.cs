//-----------------------------------------------------------------------
// <copyright file="ConvertGlobFailedException.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System;

    public class ConvertGlobFailedException : Exception
    {
        public ConvertGlobFailedException(string message)
            : base(message)
        {
        }
    }
}
