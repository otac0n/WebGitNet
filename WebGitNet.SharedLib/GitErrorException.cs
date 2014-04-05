//-----------------------------------------------------------------------
// <copyright file="GitErrorException.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System;

    public class GitErrorException : Exception
    {
        private readonly string command;
        private readonly string errorOutput;
        private readonly int exitCode;

        public GitErrorException(string command, string workingDir, int exitCode, string errorOutput)
            : base("Fatal error executing git command in '" + workingDir + "'." + Environment.NewLine + errorOutput)
        {
            this.command = command;
            this.exitCode = exitCode;
            this.errorOutput = errorOutput;
        }

        public string Command
        {
            get { return this.command; }
        }

        public string ErrorOutput
        {
            get { return this.errorOutput; }
        }

        public int ExitCode
        {
            get { return this.exitCode; }
        }
    }
}
