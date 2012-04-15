//-----------------------------------------------------------------------
// <copyright file="SearchQuery.cs" company="(none)">
//  Copyright © 2012 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet.Search
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class SearchQuery
    {
        public SearchQuery(IEnumerable<string> terms)
        {
            this.Terms = terms.ToList().AsReadOnly();
        }

        public SearchQuery(string q)
        {
            var terms = new List<string>();
            var matches = Regex.Matches(q, @"\G\s*(?:(?<bare_term>\S+)|""(?<quoted_term>(?:[^""]|"""")*)(?:""|\z))\s*");
            foreach (Match match in matches)
            {
                if (match.Groups["bare_term"].Success)
                {
                    terms.Add(match.Groups["bare_term"].Value);
                }
                else
                {
                    terms.Add(match.Groups["quoted_term"].Value.Replace("\"\"", "\""));
                }
            }

            this.Terms = terms.AsReadOnly();
        }

        public IList<string> Terms { get; private set; }
    }
}
