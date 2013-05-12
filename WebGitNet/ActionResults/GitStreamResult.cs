//-----------------------------------------------------------------------
// <copyright file="GitStreamResult.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.ActionResults
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Threading;
    using System.Web.Mvc;

    public class GitStreamResult : ActionResult
    {
        private readonly string commandFormat;
        private readonly string action;
        private readonly string repoPath;
        private readonly string userName;

        public GitStreamResult(string commandFormat, string action, string repoPath, string userName = null)
        {
            if (string.IsNullOrEmpty(commandFormat))
            {
                throw new ArgumentNullException("commandFormat");
            }

            this.commandFormat = commandFormat;

            if (string.IsNullOrEmpty(action))
            {
                throw new ArgumentNullException("action");
            }

            this.action = action;

            if (string.IsNullOrEmpty(repoPath))
            {
                throw new ArgumentNullException("repoPath");
            }

            this.repoPath = repoPath;
            this.userName = userName;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            var request = context.HttpContext.Request;
            var realRequest = System.Web.HttpContext.Current.Request;

            response.ContentType = "application/x-git-" + this.action + "-result";
            response.BufferOutput = false;

            using (var git = GitUtilities.StartWithUserName(string.Format(this.commandFormat, this.action), this.repoPath, this.userName, redirectInput: true))
            {
                var readThread = new Thread(() =>
                {
                    var readBuffer = new byte[4096];
                    int readCount;

                    Stream wrapperStream = null;
                    try
                    {
                        var input = realRequest.GetBufferlessInputStream();
                        if (request.Headers["Content-Encoding"] == "gzip")
                        {
                            input = wrapperStream = new GZipStream(input, CompressionMode.Decompress);
                        }

                        while ((readCount = input.Read(readBuffer, 0, readBuffer.Length)) > 0)
                        {
                            git.StandardInput.BaseStream.Write(readBuffer, 0, readCount);
                        }
                    }
                    finally
                    {
                        if (wrapperStream != null)
                        {
                            wrapperStream.Dispose();
                        }
                    }

                    git.StandardInput.Close();
                });
                readThread.Start();

                var writeBuffer = new byte[4096];
                int writeCount;
                byte[] copy = null;

                while ((writeCount = git.StandardOutput.BaseStream.Read(writeBuffer, 0, writeBuffer.Length)) > 0)
                {
                    if (copy == null || copy.Length != writeCount)
                    {
                        copy = new byte[writeCount];
                    }

                    for (int i = 0; i < writeCount; i++)
                    {
                        copy[i] = writeBuffer[i];
                    }

                    response.BinaryWrite(copy);
                }

                readThread.Join();
                git.WaitForExit();

                if (git.ExitCode != 0)
                {
                    response.StatusCode = 500;
                    response.SubStatusCode = git.ExitCode;
                }
            }
        }
    }
}