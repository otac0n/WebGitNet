//-----------------------------------------------------------------------
// <copyright file="LogEntry.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class LogEntry
    {
        private readonly string commitHash;
        private readonly string tree;
        private readonly IList<string> parents;
        private readonly string author;
        private readonly string authorEmail;
        private readonly DateTimeOffset authorDate;
        private readonly string committer;
        private readonly string committerEmail;
        private readonly DateTimeOffset committerDate;
        private readonly string subject;
        private readonly string body;

        public LogEntry(string commitHash, string tree, string parents, string author, string authorEmail, string authorDate, string committer, string committerEmail, string committerDate, string subject, string body)
        {
            this.commitHash = commitHash;
            this.tree = tree;
            this.parents = string.IsNullOrEmpty(parents)
                ? (IList<string>)new string[0]
                : parents.Split(' ').ToList().AsReadOnly();
            this.author = author;
            this.authorEmail = authorEmail;
            this.authorDate = DateTimeOffset.Parse(authorDate);
            this.committer = committer;
            this.committerEmail = committerEmail;
            this.committerDate = DateTimeOffset.Parse(committerDate);
            this.subject = subject;
            this.body = body;
        }

        public string CommitHash
        {
            get
            {
                return this.commitHash;
            }
        }

        public string Tree
        {
            get
            {
                return this.tree;
            }
        }

        public IList<string> Parents
        {
            get
            {
                return this.parents;
            }
        }

        public string Author
        {
            get
            {
                return this.author;
            }
        }

        public string AuthorEmail
        {
            get
            {
                return this.authorEmail;
            }
        }

        public DateTimeOffset AuthorDate
        {
            get
            {
                return this.authorDate;
            }
        }

        public string Committer
        {
            get
            {
                return this.committer;
            }
        }

        public string CommitterEmail
        {
            get
            {
                return this.committerEmail;
            }
        }

        public DateTimeOffset CommitterDate
        {
            get
            {
                return this.committerDate;
            }
        }

        public string Subject
        {
            get
            {
                return this.subject;
            }
        }

        public string Body
        {
            get
            {
                return this.body;
            }
        }
    }
}
