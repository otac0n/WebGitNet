//-----------------------------------------------------------------------
// <copyright file="AtomActionResult.cs" company="(none)">
//  Copyright © 2013 John Gietzen and the WebGit .NET Authors. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.ActionResults
{
    using System;
    using System.ServiceModel.Syndication;
    using System.Web.Mvc;
    using System.Xml;

    public class AtomActionResult : ActionResult
    {
        private readonly SyndicationFeed feed;

        public AtomActionResult(SyndicationFeed feed)
        {
            if (feed == null)
            {
                throw new ArgumentNullException("feed");
            }

            this.feed = feed;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = "application/atom+xml";

            Atom10FeedFormatter formatter = new Atom10FeedFormatter(this.feed);
            using (XmlWriter writer = XmlWriter.Create(response.Output))
            {
                formatter.WriteTo(writer);
            }
        }
    }
}