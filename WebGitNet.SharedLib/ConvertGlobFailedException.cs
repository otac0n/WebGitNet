//-----------------------------------------------------------------------
// <copyright file="ConvertGlobFailedException.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
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
