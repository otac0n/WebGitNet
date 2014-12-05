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
        private readonly string action;
        private readonly string commandFormat;
        private readonly string repoPath;

        public GitStreamResult(string commandFormat, string action, string repoPath)
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
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            var request = context.HttpContext.Request;
            var realRequest = System.Web.HttpContext.Current.Request;

            bool receivePack = action == "receive-pack";

            response.ContentType = "application/x-git-" + this.action + "-result";
            response.BufferOutput = false;

            using (var git = GitUtilities.Start(string.Format(this.commandFormat, this.action), this.repoPath, redirectInput: true))
            {
                var readThread = new Thread(() =>
                {
                    var readBuffer = new byte[4096];
                    int readCount;

                    Stream wrapperStream = null;
                    try
                    {
                        var input = realRequest.GetBufferlessInputStream(disableMaxRequestLength: true);
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

                    //Copy to smaller array because BinairyWrite usses the length of the array.
                    Array.Copy(writeBuffer, copy, writeCount);

                    response.BinaryWrite(copy);

                    //LB20140922 If uploadpack en responce lf00000000 then end of data so stop. 
                    //When using Subgit the process kees on running for a longer peiod till everuthing is pushed into Subversion. Top prevent this wait break;
                    //When not using Subgit this is still no problem because these bytes mark the end of the pack according to the protocol.
                    if (receivePack && writeCount > 8 &&
                        copy[writeCount - 9] == 10 &&
                        copy[writeCount - 8] == 48 &&
                        copy[writeCount - 7] == 48 &&
                        copy[writeCount - 6] == 48 &&
                        copy[writeCount - 5] == 48 &&
                        copy[writeCount - 4] == 48 &&
                        copy[writeCount - 3] == 48 &&
                        copy[writeCount - 2] == 48 &&
                        copy[writeCount - 1] == 48)
                        break;
                }

                readThread.Join();
                git.WaitForExit();
            }
        }
    }
}