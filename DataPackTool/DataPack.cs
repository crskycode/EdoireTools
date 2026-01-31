using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System.Text;
using System.Text.RegularExpressions;

#pragma warning disable SYSLIB1045
#pragma warning disable IDE0270

namespace DataPackTool
{
    internal class DataPack
    {
        public static void ExtractFromStream(Stream stream, string outputPath)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, true);

            var nameLength = reader.ReadInt32();

            if (nameLength < 0 && nameLength > byte.MaxValue)
            {
                throw new Exception("Invalid file format.");
            }

            var name = Encoding.UTF8.GetString(reader.ReadBytes(nameLength));

            Console.WriteLine("Name: {0}", name);

            stream.Position = (stream.Position + 3) & ~3;

            var dataLength = reader.ReadInt32();

            if (dataLength <= 0)
            {
                throw new Exception("Invalid file format.");
            }

            var signature = reader.ReadInt32();
            stream.Position -= 4;

            var extension = signature switch
            {
                0x474E5089 => ".png",
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(extension))
            {
                name += extension;
            }

            Directory.CreateDirectory(outputPath);

            var filePath = Path.Combine(outputPath, name);
            var block = new byte[0x100000];

            stream.CopyToFile(filePath, dataLength, block);

            Console.WriteLine("Done.");
        }

        public static void ExtractFromRawFile(string inputPath, string outputPath)
        {
            using var stream = File.OpenRead(inputPath);
            ExtractFromStream(stream, outputPath);
        }

        public static void ExtractFromAssetsBundle(string inputPath, string outputPath)
        {
            var assetsManager = new AssetsManager();

            var bundle = assetsManager.LoadBundleFile(inputPath);

            try
            {
                if (bundle.file.DataIsCompressed)
                {
                    throw new NotImplementedException();
                }

                var directories = bundle.file.BlockAndDirInfo.DirectoryInfos;

                foreach (var entry in directories)
                {
                    if (!Regex.IsMatch(entry.Name, "^CAB-[0-9a-z]+$"))
                    {
                        continue;
                    }

                    var cabStream = new SegmentStream(bundle.file.DataReader.BaseStream, entry.Offset, entry.DecompressedSize);
                    var cabPath = Path.Combine(bundle.path, entry.Name);
                    var cabAsset = assetsManager.LoadAssetsFile(cabStream, cabPath, false);

                    if (cabAsset == null)
                    {
                        throw new Exception("Unable to load assets.");
                    }

                    cabAsset.parentBundle ??= bundle;

                    foreach (var asset in cabAsset.file.AssetInfos)
                    {
                        var pathId = asset.PathId;
                        var offset = asset.GetAbsoluteByteOffset(cabAsset.file);
                        var length = asset.ByteSize;

                        if (pathId != 1)
                        {
                            var stream = new SegmentStream(cabAsset.file.Reader.BaseStream, offset, length);
                            ExtractFromStream(stream, outputPath);
                            return;
                        }
                    }
                }
            }
            finally
            {
                bundle.file.Close();
            }
        }

        public static bool IsAssetsBundleFile(string inputPath)
        {
            using var reader = new BinaryReader(File.OpenRead(inputPath));
            return (reader.ReadInt64() & 0xFFFFFFFFFFFFFF) == 0x53467974696E55; // UnityFS
        }
    }
}
