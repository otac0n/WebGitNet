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
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
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
            var results = Execute("log -1 --encoding=UTF-8 --format=\"format:commit %H%ntree %T%nparent %P%nauthor %an%nauthor mail %ae%ncommitter %cn%ncommitter mail %ce%nsubject %s%n%b%x00\"", repoPath, Encoding.UTF8);

            Func<string, LogEntry> parseResults = result =>
            {
                var commit = ParseResultLine("commit ", result, out result);
                var tree = ParseResultLine("tree ", result, out result);
                var parent = ParseResultLine("parent ", result, out result);
                var author = ParseResultLine("author ", result, out result);
                var authorEmail = ParseResultLine("author mail ", result, out result);
                var committer = ParseResultLine("committer ", result, out result);
                var committerEmail = ParseResultLine("committer mail ", result, out result);
                var subject = ParseResultLine("subject ", result, out result);
                var body = result;

                return new LogEntry(commit, tree, parent, author, authorEmail, committer, committerEmail, subject, body);
            };

            return (from r in results.Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries)
                    select parseResults(r)).ToList();
        }

        public static TreeView GetTreeInfo(string repoPath, string tree, string path = null)
        {
            if (string.IsNullOrEmpty(tree))
            {
                throw new ArgumentNullException("tree");
            }

            if (!Regex.IsMatch(tree, "^[-a-zA-Z0-9]+$"))
            {
                throw new ArgumentOutOfRangeException("tree", "tree mush be the id of a tree-ish object.");
            }

            path = path ?? string.Empty;
            path = path.Replace("\\", "\\\\").Replace("\"", "\\\"");
            var results = Execute(string.Format("ls-tree -l -z {0}:\"{1}\"", tree, path), repoPath, Encoding.UTF8);

            if (results.StartsWith("fatal: "))
            {
                throw new Exception(results);
            }

            Func<string, ObjectInfo> parseResults = result =>
            {
                var mode = ParseTreePart(result, "[ ]+", out result);
                var type = ParseTreePart(result, "[ ]+", out result);
                var hash = ParseTreePart(result, "[ ]+", out result);
                var size = ParseTreePart(result, "\\t+", out result);
                var name = result;

                return new ObjectInfo(
                    (ObjectType)Enum.Parse(typeof(ObjectType), type, ignoreCase: true),
                    hash,
                    size == "-" ? (int?)null : int.Parse(size),
                    name);
            };

            var objects = from r in results.Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries)
                          select parseResults(r);
            return new TreeView(tree, path, objects);
        }

        public static void CreateRepo(string repoPath)
        {
            var workingDir = Path.GetDirectoryName(repoPath);
            var newPath = repoPath.Replace("\\", "\\\\").Replace("\"", "\\\"");
            var results = Execute(string.Format("init --bare \"{0}\"", newPath), workingDir);

            var errorLines = results.Split('\n').Where(l => l.StartsWith("fatal:")).ToList();
            if (errorLines.Count > 0)
            {
                throw new CreateRepoFailedException(string.Join(results, Environment.NewLine));
            }
        }

        private static string ParseResultLine(string prefix, string result, out string rest)
        {
            var parts = result.Split(new[] { '\n' }, 2);
            rest = parts[1];
            return parts[0].Substring(prefix.Length);
        }

        private static string ParseTreePart(string result, string delimiterPattern, out string rest)
        {
            var match = Regex.Match(result, delimiterPattern);

            if (!match.Success)
            {
                rest = result;
                return null;
            }
            else
            {
                rest = result.Substring(match.Index + match.Length);
                return result.Substring(0, match.Index);
            }
        }

        [global::System.Serializable]
        public class CreateRepoFailedException : Exception
        {
            public CreateRepoFailedException()
            {
            }

            public CreateRepoFailedException(string message)
                : base(message)
            {
            }

            public CreateRepoFailedException(string message, Exception inner)
                : base(message, inner)
            {
            }

            protected CreateRepoFailedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
                : base(info, context)
            {
            }
        }
    }
}
