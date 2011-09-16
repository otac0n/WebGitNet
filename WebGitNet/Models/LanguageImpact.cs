using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGitNet.Models
{
    public class LanguageImpact
    {
        public string ContentType { get; set; }

        public int Insertions { get; set; }

        public int Deletions { get; set; }

        public int Gain { get; set; }
    }
}