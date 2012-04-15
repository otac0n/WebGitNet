//-----------------------------------------------------------------------
// <copyright file="SearchHelper.cs" company="(none)">
//  Copyright © 2012 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Search
{
    using System;
    using System.Collections.Generic;
    using Intervals;

    public static class SearchHelper
    {
        public static List<StringInterval> FindIntervals(string term, string subject)
        {
            if (string.IsNullOrEmpty(term))
            {
                throw new ArgumentNullException("term");
            }

            if (subject == null)
            {
                return null;
            }

            var results = new List<StringInterval>();
            var end = 0;

            while (end < subject.Length)
            {
                var match = subject.IndexOf(term, end, StringComparison.CurrentCultureIgnoreCase);
                if (match == -1)
                {
                    break;
                }

                end = match + term.Length;
                results.Add(new StringInterval(subject, match, term.Length));
            }

            return results;
        }
    }
}
