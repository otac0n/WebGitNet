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
        public static Encoding DefaultEncoding
        {
            get { return Encoding.GetEncoding(28591); }
        }

        public static string Execute(string command, string workingDir, Encoding outputEncoding = null)
        {
            using (var git = Start(command, workingDir, redirectInput: false, outputEncoding: outputEncoding))
            {
                var result = git.StandardOutput.ReadToEnd();
                git.WaitForExit();
                return result;
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
                StandardOutputEncoding = outputEncoding ?? DefaultEncoding,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            return Process.Start(startInfo);
        }

        public static void UpdateServerInfo(string repoPath)
        {
            Execute("update-server-info", repoPath);
        }

        public static List<LogEntry> GetLogEntries(string repoPath, int count, string @object = null)
        {
            @object = @object ?? "HEAD";
            var results = Execute(string.Format("log -n {0} --encoding=UTF-8 -z --format=\"format:commit %H%ntree %T%nparent %P%nauthor %an%nauthor mail %ae%nauthor date %aD%ncommitter %cn%ncommitter mail %ce%ncommitter date %cD%nsubject %s%n%b%x00\" {1}", count, @object), repoPath, Encoding.UTF8);

            Func<string, LogEntry> parseResults = result =>
            {
                var commit = ParseResultLine("commit ", result, out result);
                var tree = ParseResultLine("tree ", result, out result);
                var parent = ParseResultLine("parent ", result, out result);
                var author = ParseResultLine("author ", result, out result);
                var authorEmail = ParseResultLine("author mail ", result, out result);
                var authorDate = ParseResultLine("author date ", result, out result);
                var committer = ParseResultLine("committer ", result, out result);
                var committerEmail = ParseResultLine("committer mail ", result, out result);
                var committerDate = ParseResultLine("committer date ", result, out result);
                var subject = ParseResultLine("subject ", result, out result);
                var body = result;

                return new LogEntry(commit, tree, parent, author, authorEmail, authorDate, committer, committerEmail, committerDate, subject, body);
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

        public static Process StartGetBlob(string repoPath, string tree, string path)
        {
            if (string.IsNullOrEmpty(tree))
            {
                throw new ArgumentNullException("tree");
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            if (!Regex.IsMatch(tree, "^[-a-zA-Z0-9]+$"))
            {
                throw new ArgumentOutOfRangeException("tree", "tree mush be the id of a tree-ish object.");
            }

            path = path.Replace("\\", "\\\\").Replace("\"", "\\\"");
            return Start(string.Format("show {0}:\"{1}\"", tree, path), repoPath, redirectInput: false);
        }

        public static MemoryStream GetBlob(string repoPath, string tree, string path)
        {
            MemoryStream blob = null;
            try
            {
                blob = new MemoryStream();
                using (var git = StartGetBlob(repoPath, tree, path))
                {
                    var buffer = new byte[1048576];
                    var readCount = 0;
                    while ((readCount = git.StandardOutput.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        blob.Write(buffer, 0, readCount);
                    }
                }

                blob.Seek(0, SeekOrigin.Begin);

                var tempBlob = blob;
                blob = null;
                return tempBlob;
            }
            finally
            {
                if (blob != null)
                {
                    blob.Dispose();
                }
            }
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
