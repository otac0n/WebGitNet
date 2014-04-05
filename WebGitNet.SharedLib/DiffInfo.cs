//-----------------------------------------------------------------------
// <copyright file="DiffInfo.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class DiffInfo
    {
        private readonly bool added;
        private readonly bool copied;
        private readonly bool deleted;
        private readonly string destinationFile;
        private readonly IList<string> headers;
        private readonly IList<string> lines;
        private readonly bool renamed;
        private readonly string sourceFile;

        public DiffInfo(IList<string> lines)
        {
            if (lines == null)
            {
                throw new ArgumentNullException("lines");
            }

            Func<string, bool> isHeader = l => !l.StartsWith("@") && !l.StartsWith("Binary files");

            this.headers = lines.TakeWhile(isHeader).ToList().AsReadOnly();
            this.lines = lines.Skip(this.headers.Count).ToList().AsReadOnly();

            var match = Regex.Match(this.headers[0], "^diff --git a:/(?<src>.*?) b:/(?<dst>.*)$");
            if (match.Success)
            {
                this.sourceFile = match.Groups["src"].Value;
                this.destinationFile = match.Groups["dst"].Value;
            }

            this.renamed = this.headers.Any(h => h.StartsWith("rename from"));
            this.copied = this.headers.Any(h => h.StartsWith("copy from"));
            this.added = this.headers.Any(h => h.StartsWith("new file"));
            this.deleted = this.headers.Any(h => h.StartsWith("deleted file"));
        }

        public bool Added
        {
            get { return this.added; }
        }

        public bool Copied
        {
            get { return this.copied; }
        }

        public bool Deleted
        {
            get { return this.deleted; }
        }

        public string DestinationFile
        {
            get { return this.destinationFile; }
        }

        public IList<string> Headers
        {
            get { return this.headers; }
        }

        public IList<string> Lines
        {
            get { return this.lines; }
        }

        public bool Renamed
        {
            get { return this.renamed; }
        }

        public string SourceFile
        {
            get { return this.sourceFile; }
        }
    }
}
