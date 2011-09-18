namespace WebGitNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class PathUtilities
    {
        public static Regex GlobToRegex(string glob)
        {
            var tokenized = Regex.Matches(glob, @"\G(?:(?<literal>[^?*[]+)|(?<wildcard>[*?])|(?<class>\[!?\]?([^][]|\[(:[^]:]*:|\.[^].]*\.|=[^]=]*=)\])*\]))");
            var text = new StringBuilder(glob.Length * 2).Append(@"\A");

            if (tokenized.Count == 0)
            {
                throw new ConvertGlobFailedException("Syntax error at character 0.");
            }

            var last = tokenized.Cast<Match>().Last();
            var index = last.Index + last.Length;
            if (index != glob.Length)
            {
                throw new ConvertGlobFailedException("Syntax error at character " + index + ".");
            }

            foreach (Match token in tokenized)
            {
                var literal = token.Groups["literal"];
                var wildcard = token.Groups["wildcard"];
                var charClass = token.Groups["class"];

                if (literal.Success)
                {
                    text.Append(Regex.Escape(literal.Value));
                }
                else if (wildcard.Success)
                {
                    switch (wildcard.Value)
                    {
                        case "?": text.Append(@"[^/]"); break;
                        case "*": text.Append(@"[^/]*"); break;
                    }
                }
                else if (charClass.Success)
                {
                    var content = charClass.Value;
                    content = content.Substring(1, content.Length - 2);

                    bool negate = false;

                    if (content.StartsWith("!"))
                    {
                        negate = true;
                        content = content.Substring(1);
                    }

                    var chunks = Regex.Matches(content, @"(?<literal>[^][])|\[(?::(?<named>[^]:]*):|\.(?<collating>[^].]*)\.|=(?<equivalence>[^]=]*)=)\]");

                    text.Append("[");
                    text.Append(negate ? "^" : "");

                    foreach (Match chunk in chunks)
                    {
                        literal = chunk.Groups["literal"];
                        var named = chunk.Groups["named"];

                        if (literal.Success)
                        {
                            if (literal.Value.Contains("/"))
                            {
                                throw new ConvertGlobFailedException("Forward-slant characters are not valid in glob character classes.");
                            }

                            text.Append(literal.Value.Replace(@"\", @"\\").Replace(@"]", @"\]"));
                        }
                        else if (named.Success)
                        {
                            switch (named.Value.ToLowerInvariant())
                            {
                                case "alnum": text.Append(@"\p{Lu}\p{Ll}\p{Lt}\p{Nd}"); break;
                                case "alpha": text.Append(@"\p{Lu}\p{Ll}\p{Lt}"); break;
                                case "blank": text.Append(@"\f\n\r\t\v\x85\p{Z}"); break;
                                case "cntrl": text.Append(@"\p{Cc}"); break;
                                case "digit": text.Append(@"\p{Nd}"); break;
                                case "lower": text.Append(@"\p{Ll}"); break;
                                case "space": text.Append(@"\p{Zs}"); break;
                                case "upper": text.Append(@"\p{Lu}"); break;
                                case "xdigit": text.Append(@"\p{Nd}a-fA-F"); break;
                                default: throw new ConvertGlobFailedException("The named character class '" + named.Value + "' is supported.");
                            }
                        }
                        else
                        {
                            throw new ConvertGlobFailedException("Collating and equivalence classes are not supported.");
                        }
                    }

                    text.Append("]");
                }
            }

            return new Regex(text.Append(@"\z").ToString(), RegexOptions.Compiled);
        }
    }
}
