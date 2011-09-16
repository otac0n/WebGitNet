//-----------------------------------------------------------------------
// <copyright file="GitFileResult.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.ActionResults
{
    using System.Web.Mvc;

    public class GitFileResult : ActionResult
    {
        private readonly string repoPath;
        private readonly string tree;
        private readonly string path;
        private readonly string contentType;

        public GitFileResult(string repoPath, string tree, string path, string contentType = null)
        {
            this.repoPath = repoPath;
            this.tree = tree;
            this.path = path;
            this.contentType = string.IsNullOrEmpty(contentType) ? "application/octet-stream" : contentType;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            var request = context.HttpContext.Request;

            response.ContentType = this.contentType;
            response.AddHeader("Content-disposition", "attachment");
            response.Buffer = false;
            response.BufferOutput = false;

            using (var git = GitUtilities.StartGetBlob(this.repoPath, this.tree, this.path))
            {
                var writeBuffer = new char[4194304];
                int writeCount;
                while ((writeCount = git.StandardOutput.ReadBlock(writeBuffer, 0, writeBuffer.Length)) > 0)
                {
                    var bytes = GitUtilities.DefaultEncoding.GetBytes(writeBuffer, 0, writeCount);
                    response.BinaryWrite(bytes);
                }

                git.WaitForExit();
            }
        }
    }
}