using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace WebGitNet
{
    public static class RenameFileParser
    {
        private enum Terminal
        {
            Field,
            SearchType,
            Assignment,
            Seperator,
            Definition,
            String,
            Whitespace,
            EndOfLine,
        }

        private static readonly Dictionary<Terminal, string> terminals = new Dictionary<Terminal, string>
        {
            { Terminal.Field, "name|email" },
            { Terminal.SearchType, "[=%~]=" },
            { Terminal.Assignment, "=" },
            { Terminal.Seperator, "," },
            { Terminal.Definition, ":" },
            { Terminal.String, @"""((?:[^""]|"""")+)""" },
            { Terminal.Whitespace, @"\s+" },
            { Terminal.EndOfLine, @"$" },
        };

        public static List<RenameEntry> Parse(string[] lines)
        {
            return lines.Select(l => Parse(l)).Where(l => l != null).ToList();
        }

        private static RenameEntry Parse(string line)
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
            {
                return null;
            }

            RenameField sourceField = RenameField.None;
            string operation = null;
            string match = null;
            RenameField destinationField1 = RenameField.None;
            string destinationValue1 = null;
            RenameField destinationField2 = RenameField.None;
            string destinationValue2 = null;

            Accept(Terminal.Whitespace, ref line);

            sourceField = (RenameField)Enum.Parse(typeof(RenameField), Require(Terminal.Field, ref line), true);
            Accept(Terminal.Whitespace, ref line);
            operation = Require(Terminal.SearchType, ref line);
            Accept(Terminal.Whitespace, ref line);
            match = Require(Terminal.String, ref line);
            match = match.Substring(1, match.Length - 2).Replace("\"\"", "\"");

            Accept(Terminal.Whitespace, ref line);
            Require(Terminal.Definition, ref line);
            Accept(Terminal.Whitespace, ref line);

            destinationField1 = (RenameField)Enum.Parse(typeof(RenameField), Require(Terminal.Field, ref line), true);
            Accept(Terminal.Whitespace, ref line);
            Require(Terminal.Assignment, ref line);
            Accept(Terminal.Whitespace, ref line);
            destinationValue1 = Require(Terminal.String, ref line);
            destinationValue1 = destinationValue1.Substring(1, destinationValue1.Length - 2).Replace("\"\"", "\"");

            Accept(Terminal.Whitespace, ref line);
            if (Accept(Terminal.Seperator, ref line) != null)
            {
                destinationField2 = (RenameField)Enum.Parse(typeof(RenameField), Require(Terminal.Field, ref line), true);
                Accept(Terminal.Whitespace, ref line);
                Require(Terminal.Assignment, ref line);
                Accept(Terminal.Whitespace, ref line);
                destinationValue2 = Require(Terminal.String, ref line);
                destinationValue2 = destinationValue2.Substring(1, destinationValue2.Length - 2).Replace("\"\"", "\"");
            }

            Accept(Terminal.Whitespace, ref line);
            Require(Terminal.EndOfLine, ref line);

            if (destinationField1 == destinationField2)
            {
                throw new RenameFileSyntaxException("Duplicate destinations are invalid.");
            }

            RenameEntry.Destination[] destinations;
            if (destinationField2 == RenameField.None)
            {
                destinations = new[] {
                    new RenameEntry.Destination { Field = destinationField1, Replacement = destinationValue1 },
                };
            }
            else
            {
                destinations = new[] {
                    new RenameEntry.Destination { Field = destinationField1, Replacement = destinationValue1 },
                    new RenameEntry.Destination { Field = destinationField2, Replacement = destinationValue2 },
                };
            }

            RenameStyle style = RenameStyle.Exact;
            switch (operation)
            {
                case "~=": style = RenameStyle.CaseInsensitive; break;
                case "%=": style = RenameStyle.Regex; break;
            }

            return new RenameEntry
            {
                RenameStyle = style,
                SourceField = sourceField,
                Match = match,
                Destinations = destinations,
            };
        }

        private static string Accept(Terminal terminal, ref string subject)
        {
            var regex = terminals[terminal];

            var match = Regex.Match(subject, "^" + regex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (match.Success)
            {
                subject = subject.Substring(match.Length);
                return match.Value;
            }
            else
            {
                return null;
            }
        }

        private static string Require(Terminal terminal, ref string subject)
        {
            var match = Accept(terminal, ref subject);
            if (match == null)
            {
                throw new RenameFileSyntaxException("Expected " + terminal + ".");
            }

            return match;
        }
    }
}