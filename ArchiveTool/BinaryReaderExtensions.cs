using System;
using System.Collections.Generic;
using System.Text;

namespace ArchiveTool
{
    internal static class BinaryReaderExtensions
    {
        public static string ReadShortString(this BinaryReader reader)
        {
            var length = reader.ReadByte();

            if (length == 0)
            {
                return string.Empty;
            }

            var buffer = reader.ReadBytes(length);

            return Encoding.UTF8.GetString(buffer);
        }
    }
}
