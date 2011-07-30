//-----------------------------------------------------------------------
// <copyright file="LogEntry.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Models
{
    public class LogEntry
    {
        private readonly string commit;
        private readonly string tree;
        private readonly string parent;
        private readonly string author;
        private readonly string authorEmail;
        private readonly string committer;
        private readonly string committerEmail;
        private readonly string subject;
        private readonly string body;

        public LogEntry(string commit, string tree, string parent, string author, string authorEmail, string committer, string committerEmail, string subject, string body)
        {
            this.commit = commit;
            this.tree = tree;
            this.parent = parent;
            this.author = author;
            this.authorEmail = authorEmail;
            this.committer = committer;
            this.committerEmail = committerEmail;
            this.subject = subject;
            this.body = body;
        }

        public string Commit
        {
            get
            {
                return this.commit;
            }
        }

        public string Tree
        {
            get
            {
                return this.tree;
            }
        }

        public string Parent
        {
            get
            {
                return this.parent;
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
