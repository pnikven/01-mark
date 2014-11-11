using System;
using System.IO;
using System.Reflection;

namespace MarkdownProcessor
{
    public class Program
    {
        static void Main(string[] args)
        {
            var filename = GetFirstParameter(args);

            var fileContent = File.ReadAllText(filename);

            var markdownParser = new MarkdownParser();
            var result = markdownParser.Parse(fileContent);

            File.WriteAllText(FormResultFilename("result.html"), result);
        }

        private static string GetFirstParameter(string[] consoleArguments)
        {
            if (consoleArguments.Length != 0) return consoleArguments[0];
            throw new Exception("File name must be provided");
        }

        private static string FormResultFilename(string filename)
        {
            var programDirecory = GetExecutingAssemblyDirectory();
            return Path.Combine(programDirecory, filename);
        }

        private static string GetExecutingAssemblyDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

    }
}