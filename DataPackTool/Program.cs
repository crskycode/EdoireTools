using System.Text;

namespace DataPackTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("DataPackTool");
                Console.WriteLine("  created by Crsky");
                Console.WriteLine();
                Console.WriteLine("Usage:");
                Console.WriteLine("  Extract : DataPackTool -e -in [file] -out [folder]");
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");

                Environment.ExitCode = 1;
                Console.ReadKey();

                return;
            }

            var parsedArgs = CommandLineParser.ParseArguments(args);

            CommandLineParser.EnsureArguments(parsedArgs, "-in", "-out");

            var inputPath = Path.GetFullPath(parsedArgs["-in"]);
            var outputPath = Path.GetFullPath(parsedArgs["-out"]);

            if (parsedArgs.ContainsKey("-e"))
            {
                if (DataPack.IsAssetsBundleFile(inputPath))
                {
                    DataPack.ExtractFromAssetsBundle(inputPath, outputPath);
                }
                else
                {
                    DataPack.ExtractFromRawFile(inputPath, outputPath);
                }

                return;
            }
        }
    }
}
