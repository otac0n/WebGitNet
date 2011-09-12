using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebGitNet
{
    public class RenameFileSyntaxException : Exception
    {
        public RenameFileSyntaxException(string message)
            : base(message)
        {
        }
    }
}