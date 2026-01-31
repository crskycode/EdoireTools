using System;
using System.Collections.Generic;
using System.Text;

namespace ArchiveTool
{
    internal class ArchiveFile
    {
        public static void Extract(string filePath, string outputPath)
        {
            using var stream = File.OpenRead(filePath);
            using var reader = new BinaryReader(stream);

            // Check header

            var signature = reader.ReadInt64();

            if (signature != 0x3030304843524140) // @ARCH000
            {
                throw new Exception("Invalid file signature.");
            }

            // Read index

            stream.Position = stream.Length - 8;
            var indexPos = reader.ReadInt64();

            stream.Position = indexPos;
            var count = reader.ReadInt32();

            if (count < 0)
            {
                throw new Exception("Invalid file format.");
            }

            Console.WriteLine("Count: {0}", count);

            var entries = new List<Entry>(count);

            for (var i = 0; i < count; i++)
            {
                var name = reader.ReadShortString();
                var position = reader.ReadInt64();
                var length = reader.ReadInt64();
                reader.ReadByte(); // 0x4E
                var path = reader.ReadShortString();

                entries.Add(new Entry
                {
                    Name = name,
                    Path = path.Replace('/', Path.DirectorySeparatorChar),
                    Position = position,
                    Length = length,
                });
            }

            // Extract

            Directory.CreateDirectory(outputPath);

            var block = new byte[0x100000];

            foreach (var entry in entries)
            {
                var entryPath = Path.Combine(entry.Path, entry.Name + ".unity3d");

                // Fix path

                if (entryPath.StartsWith(Path.DirectorySeparatorChar))
                {
                    entryPath = entryPath.Substring(1);
                }

                Console.WriteLine("Extract: {0}", entryPath);

                stream.Position = entry.Position;

                // Check entry header

                if ((reader.ReadInt64() & 0xFFFFFFFFFFFFFF) != 0x53467974696E55) // UnityFS
                {
                    throw new Exception("Invalid entry format.");
                }

                // Copy

                var outFilePath = Path.Combine(outputPath, entryPath);
                var outDirPath = Path.GetDirectoryName(outFilePath) ?? string.Empty;

                Directory.CreateDirectory(outDirPath);

                stream.Position = entry.Position;
                stream.CopyToFile(outFilePath, entry.Length, block);
            }

            Console.WriteLine("Done.");
        }

        public static void Create(string rootPath, string filePath)
        {
            // List files

            var files = Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories);

            var entries = new List<Entry>(files.Length);

            foreach (var path in files)
            {
                var entry = new Entry
                {
                    Path = path,
                    Name = Path.GetRelativePath(rootPath, path)
                };

                entries.Add(entry);
            }

            entries = entries.OrderBy(x => x.Name, FileNameComparer.Instance).ToList();

            // Create

            using var writer = new BinaryWriter(File.Create(filePath));

            // Write header

            writer.Write(0x3030304843524140); // @ARCH000
            writer.Write((byte)0xE);
            writer.Write(Encoding.UTF8.GetBytes(DateTime.Now.ToString("yyyyMMddHHmmss")));

            // Add files

            var block = new byte[0x100000];

            foreach (var entry in entries)
            {
                Console.WriteLine("Add: {0}", entry.Name);

                entry.Position = writer.BaseStream.Position;
                entry.Length = writer.BaseStream.CopyFromFile(entry.Path, block);
            }

            // Write index

            Console.WriteLine("Write index");

            var indexPos = writer.BaseStream.Position;

            writer.Write(entries.Count);

            foreach (var entry in entries)
            {
                var name = Path.GetFileName(entry.Name);

                // Remove extension

                var extension = Path.GetExtension(entry.Name).ToLowerInvariant();

                if (extension == ".unity3d")
                {
                    name = Path.GetFileNameWithoutExtension(entry.Name);
                }

                // Normalize path

                var path = (Path.GetDirectoryName(entry.Name) ?? string.Empty)
                    .Replace(Path.DirectorySeparatorChar, '/');

                if (!string.IsNullOrEmpty(path))
                {
                    path = "/" + path;
                }

                // Write entry

                writer.WriteShortString(name);
                writer.Write(entry.Position);
                writer.Write(entry.Length);
                writer.Write((byte)0x4E);
                writer.WriteShortString(path);
            }

            writer.Write(indexPos);
            writer.Flush();

            Console.WriteLine("Done.");
        }

        class Entry
        {
            public string Name { get; set; } = string.Empty;
            public string Path { get; set; } = string.Empty;
            public long Position { get; set; }
            public long Length { get; set; }
        }
    }
}
