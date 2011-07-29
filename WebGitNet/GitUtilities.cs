//-----------------------------------------------------------------------
// <copyright file="GitUtilities.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System.Diagnostics;
    using System.Web.Configuration;

    public static class GitUtilities
    {
        public static string Execute(string command, string workingDir)
        {
            var git = WebConfigurationManager.AppSettings["GitCommand"];
            var startInfo = new ProcessStartInfo(git, command)
            {
                WorkingDirectory = workingDir,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using (var process = Process.Start(startInfo))
            {
                return process.StandardOutput.ReadToEnd();
            }
        }

        public static void UpdateServerInfo(string repoPath)
        {
            Execute("update-server-info", repoPath);
        }
    }
}