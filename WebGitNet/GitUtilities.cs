//-----------------------------------------------------------------------
// <copyright file="GitUtilities.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System.Diagnostics;
    using System.Text;
    using System.Web.Configuration;

    public static class GitUtilities
    {
        public static string Execute(string command, string workingDir)
        {
            using (var process = Start(command, workingDir, redirectInput: false))
            {
                return process.StandardOutput.ReadToEnd();
            }
        }

        public static Process Start(string command, string workingDir, bool redirectInput = false)
        {
            var git = WebConfigurationManager.AppSettings["GitCommand"];
            var startInfo = new ProcessStartInfo(git, command)
            {
                WorkingDirectory = workingDir,
                RedirectStandardInput = redirectInput,
                RedirectStandardOutput = true,
                StandardOutputEncoding = Encoding.GetEncoding(1252),
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            return Process.Start(startInfo);
        }

        public static void UpdateServerInfo(string repoPath)
        {
            Execute("update-server-info", repoPath);
        }
    }
}
