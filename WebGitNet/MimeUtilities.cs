//-----------------------------------------------------------------------
// <copyright file="MimeUtilities.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Microsoft.Win32;

    public class MimeUtilities
    {
        private static readonly Dictionary<string, string> knownExtensions = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> knownNames = new Dictionary<string, string>();

        private static readonly List<KeyValuePair<string, string>> exactLangMatches = new List<KeyValuePair<string, string>>();
        private static readonly List<KeyValuePair<string, Regex>> regexLangMatches = new List<KeyValuePair<string, Regex>>();

        static MimeUtilities()
        {
            var assemblyPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            try
            {
                foreach (var line in File.ReadAllLines(Path.Combine(assemblyPath, "mime-types.txt")))
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    var parts = line.Split(new[] { ':' }, 2);
                    var file = parts[0].ToLowerInvariant();
                    var type = parts[1].ToLowerInvariant();

                    if (file.StartsWith("."))
                    {
                        knownExtensions[file] = type;
                    }
                    else
                    {
                        knownNames[file] = type;
                    }
                }
            }
            catch (IOException)
            {
            }

            try
            {
                foreach (var line in File.ReadAllLines(Path.Combine(assemblyPath, "shjs-languages.txt")))
                {
                    if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                    {
                        continue;
                    }

                    var index = line.IndexOfAny(new[] { '=', '%', '@' });
                    var lang = line.Substring(0, index);
                    var rest = line.Substring(index + 1);

                    var op = line[index];

                    if (op == '=')
                    {
                        exactLangMatches.Add(new KeyValuePair<string, string>(lang, rest));
                    }
                    else if (op == '@')
                    {
                        exactLangMatches.Add(new KeyValuePair<string, string>(lang, "text/x-" + rest));
                        exactLangMatches.Add(new KeyValuePair<string, string>(lang, "text/x-" + rest + "-source"));
                        exactLangMatches.Add(new KeyValuePair<string, string>(lang, "text/x-" + rest + "-src"));
                        exactLangMatches.Add(new KeyValuePair<string, string>(lang, "application/x-" + rest));
                        exactLangMatches.Add(new KeyValuePair<string, string>(lang, "application/x-" + rest + "-source"));
                        exactLangMatches.Add(new KeyValuePair<string, string>(lang, "application/x-" + rest + "-src"));
                    }
                    else if (op == '%')
                    {
                        regexLangMatches.Add(new KeyValuePair<string, Regex>(lang, new Regex(rest)));
                    }
                }
            }
            catch (IOException)
            {
            }
        }

        public static string GetMimeType(string fileName)
        {
            string value;

            fileName = Path.GetFileName(fileName.ToLowerInvariant());
            if (knownNames.TryGetValue(fileName, out value))
            {
                return value;
            }

            string extension = Path.GetExtension(fileName);

            if (!string.IsNullOrEmpty(extension))
            {
                if (knownExtensions.TryGetValue(extension, out value))
                {
                    return value;
                }

                value = Registry.GetValue(@"HKEY_CLASSES_ROOT\" + extension, "ContentType", string.Empty) as string;

                if (!string.IsNullOrEmpty(value))
                {
                    return value.ToLowerInvariant();
                }

                value = Registry.GetValue(@"HKEY_CLASSES_ROOT\" + extension, "Content Type", string.Empty) as string;

                if (!string.IsNullOrEmpty(value))
                {
                    return value.ToLowerInvariant();
                }

                value = Registry.GetValue(@"HKEY_CLASSES_ROOT\" + extension, "PerceivedType", string.Empty) as string;

                if (!string.IsNullOrEmpty(value))
                {
                    return value.ToLowerInvariant() + "/unknown";
                }
            }

            return "application/octet-stream";
        }

        public static string GetLanguage(string mimeType)
        {
            var lang = (from m in exactLangMatches
                        where m.Value == mimeType
                        select m.Key).FirstOrDefault();

            if (lang != null)
            {
                return lang;
            }

            lang = (from m in regexLangMatches
                    where m.Value.IsMatch(mimeType)
                    select m.Key).FirstOrDefault();

            return lang;
        }
    }
}
