using System;
using System.Collections.Generic;
using System.Text;

namespace ArchiveTool
{
    internal static class BinaryWriterExtensions
    {
        public static void WriteShortString(this BinaryWriter writer, string value)
        {
            if (value.Length > byte.MaxValue)
            {
                throw new Exception("String too long.");
            }

            if (value.Length == 0)
            {
                writer.Write((byte)0);
                return;
            }

            var buffer = Encoding.UTF8.GetBytes(value);
            var length = (byte)buffer.Length;

            writer.Write(length);
            writer.Write(buffer);
        }
    }
}
