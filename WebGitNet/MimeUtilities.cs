//-----------------------------------------------------------------------
// <copyright file="MimeUtilities.cs" company="(none)">
//  Copyright © 2011 John Gietzen. All rights reserved.
// </copyright>
// <author>John Gietzen</author>
//-----------------------------------------------------------------------

namespace WebGitNet
{
    using System.IO;
    using Microsoft.Win32;

    public class MimeUtilities
    {
        public static string GetMimeType(string fileName)
        {
            string extension = Path.GetExtension(fileName);

            if (!string.IsNullOrEmpty(extension))
            {
                var value = Registry.GetValue(@"HKEY_CLASSES_ROOT\" + extension, "ContentType", string.Empty) as string;

                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }

                value = Registry.GetValue(@"HKEY_CLASSES_ROOT\" + extension, "Content Type", string.Empty) as string;

                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }

                value = Registry.GetValue(@"HKEY_CLASSES_ROOT\" + extension, "PerceivedType", string.Empty) as string;

                if (!string.IsNullOrEmpty(value))
                {
                    return value + "/unknown";
                }
            }

            return "application/octet-stream";
        }
    }
}
