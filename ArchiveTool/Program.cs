using System.Text;

namespace ArchiveTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("ArchiveTool");
                Console.WriteLine("  created by Crsky");
                Console.WriteLine();
                Console.WriteLine("Usage:");
                Console.WriteLine("  Extract : ArchiveTool -e -in [input.arc] -out [folder]");
                Console.WriteLine("  Create  : ArchiveTool -c -in [folder] -out [output.arc]");
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
                ArchiveFile.Extract(inputPath, outputPath);
                return;
            }

            if (parsedArgs.ContainsKey("-c"))
            {
                ArchiveFile.Create(inputPath, outputPath);
                return;
            }
        }
    }
}
