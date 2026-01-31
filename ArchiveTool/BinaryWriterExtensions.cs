using System;
using System.Collections.Generic;
using System.Text;

namespace ArchiveTool
{
    internal static class BinaryWriterExtensions
    {
        public static void WriteShortString(this BinaryWriter writer, string value)
        {
            if (value.Length == 0)
            {
                writer.Write((byte)0);
                return;
            }

            var buffer = Encoding.UTF8.GetBytes(value);

            if (buffer.Length > byte.MaxValue)
            {
                throw new Exception("String too long.");
            }

            var length = (byte)buffer.Length;

            writer.Write(length);
            writer.Write(buffer);
        }
    }
}
