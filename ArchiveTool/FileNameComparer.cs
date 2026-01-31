using System;
using System.Collections.Generic;
using System.Text;

namespace ArchiveTool
{
    internal class FileNameComparer : IComparer<string>
    {
        public static readonly FileNameComparer Instance = new();

        public int Compare(string? x, string? y)
        {
            x ??= string.Empty;
            y ??= string.Empty;

            var i = 0;
            var j = 0;

            while (i < x.Length && j < y.Length)
            {
                if (x[i] != y[j])
                {
                    break;
                }

                i++;
                j++;
            }

            var a = i < x.Length ? x[i] : 0;
            var b = j < y.Length ? y[j] : 0;

            // Let the underscore greater than the lowercase letter
            if (a == '_' && (b >= 'a' && b <= 'z'))
            {
                return 1;
            }
            else if (b == '_' && (a >= 'a' && a <= 'z'))
            {
                return -1;
            }

            return a - b;
        }
    }
}
