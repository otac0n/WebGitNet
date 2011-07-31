//-----------------------------------------------------------------------
// <copyright file="GitCommandResult.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.ActionResults
{
    using System;
    using System.Web.Mvc;

    public class GitCommandResult : ActionResult
    {
        private readonly string commandFormat;
        private readonly string service;
        private readonly string workingDir;

        public GitCommandResult(string commandFormat, string service, string workingDir)
        {
            if (string.IsNullOrEmpty(commandFormat))
            {
                throw new ArgumentNullException("commandFormat");
            }

            this.commandFormat = commandFormat;

            if (string.IsNullOrEmpty(service))
            {
                throw new ArgumentNullException("service");
            }

            this.service = service;

            if (string.IsNullOrEmpty(workingDir))
            {
                throw new ArgumentNullException("workingDir");
            }

            this.workingDir = workingDir;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;

            var commandResult = GitUtilities.Execute(string.Format(this.commandFormat, this.service), this.workingDir);

            response.StatusCode = 200;
            response.ContentType = "application/x-git-" + this.service + "-advertisement";
            response.ContentEncoding = GitUtilities.DefaultEncoding;
            response.Write(PacketFormat(string.Format("# service=git-{0}\n", this.service)));
            response.Write(PacketFlush());
            response.Write(commandResult);
        }

        private static string PacketFormat(string packet)
        {
            return (packet.Length + 4).ToString("X").PadLeft(4, '0') + packet;
        }

        private static string PacketFlush()
        {
            return "0000";
        }
    }
}