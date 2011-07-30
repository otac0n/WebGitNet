//-----------------------------------------------------------------------
// <copyright file="GitUtilities.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Web.Configuration;
    using WebGitNet.Models;

    public static class GitUtilities
    {
        public static string Execute(string command, string workingDir, Encoding outputEncoding = null)
        {
            using (var process = Start(command, workingDir, redirectInput: false, outputEncoding: outputEncoding))
            {
                return process.StandardOutput.ReadToEnd();
            }
        }

        public static Process Start(string command, string workingDir, bool redirectInput = false, Encoding outputEncoding = null)
        {
            var git = WebConfigurationManager.AppSettings["GitCommand"];
            var startInfo = new ProcessStartInfo(git, command)
            {
                WorkingDirectory = workingDir,
                RedirectStandardInput = redirectInput,
                RedirectStandardOutput = true,
                StandardOutputEncoding = outputEncoding ?? Encoding.GetEncoding(1252),
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            return Process.Start(startInfo);
        }

        public static void UpdateServerInfo(string repoPath)
        {
            Execute("update-server-info", repoPath);
        }

        public static List<LogEntry> GetLogEntries(string repoPath, int count)
        {
            var results = Execute("log -1 --encoding=UTF-8 --format=\"format:commit %H%ntree %T%nparent %P%nauthor %an%nauthor mail %ae%ncommitter %cn%ncommitter mail %ce%n%B%x00\"", repoPath, Encoding.UTF8);

            Func<string, LogEntry> parseResults = result =>
            {
                var commit = ParseResultLine("commit ", result, out result);
                var tree = ParseResultLine("tree ", result, out result);
                var parent = ParseResultLine("parent ", result, out result);
                var author = ParseResultLine("author ", result, out result);
                var authorEmail = ParseResultLine("author mail ", result, out result);
                var committer = ParseResultLine("committer ", result, out result);
                var committerEmail = ParseResultLine("committer mail ", result, out result);
                var message = result;

                return new LogEntry(commit, tree, parent, author, authorEmail, committer, committerEmail, message);
            };

            return (from r in results.Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries)
                    select parseResults(r)).ToList();
        }

        private static string ParseResultLine(string prefix, string result, out string rest)
        {
            var parts = result.Split(new[] { '\n' }, 2);
            rest = parts[1];
            Debug.Assert(parts[0].StartsWith(prefix));
            return parts[0].Substring(prefix.Length);
        }
    }
}
