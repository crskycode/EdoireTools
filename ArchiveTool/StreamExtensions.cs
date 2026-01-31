using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace ArchiveTool
{
    internal static class StreamExtensions
    {
        /// <summary>
        /// Reads the bytes from the current stream and writes them to new file.
        /// </summary>
        /// <param name="source">Source stream.</param>
        /// <param name="destination">Destination file path.</param>
        /// <param name="length">Number of bytes to be copied.</param>
        /// <param name="block">An array used to avoid allocating buffers multiple times.</param>
        /// <exception cref="EndOfStreamException"></exception>
        public static void CopyToFile(this Stream source, string destination, long length, byte[] block)
        {
            using var output = File.Create(destination);

            while (length > 0)
            {
                var count = (int)Math.Min(length, block.Length);

                if (source.Read(block, 0, count) != count)
                {
                    throw new EndOfStreamException();
                }

                output.Write(block, 0, count);

                length -= count;
            }

            output.Flush();
        }

        /// <summary>
        /// Reads the bytes from the source file and writes them to current stream.
        /// </summary>
        /// <param name="destination">Destination stream.</param>
        /// <param name="source">Source file path.</param>
        /// <param name="block">An array used to avoid allocating buffers multiple times.</param>
        /// <returns>Number of bytes in source file.</returns>
        /// <exception cref="EndOfStreamException"></exception>
        public static long CopyFromFile(this Stream destination, string source, byte[] block)
        {
            using var input = File.OpenRead(source);

            var length = input.Length;

            while (length > 0)
            {
                var count = (int)Math.Min(length, block.Length);

                if (input.Read(block, 0, count) != count)
                {
                    throw new EndOfStreamException();
                }

                destination.Write(block, 0, count);

                length -= count;
            }

            destination.Flush();

            return input.Length;
        }
    }
}
