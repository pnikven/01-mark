namespace MarkdownProcessor
{
    public class Program
    {
        static void Main(string[] args)
        {
            IParameterManager parameterManager=new ConsoleParameterManager();
            var filename = parameterManager.GetFirstParameter(args);

            IFileManager fileManager = new FileManager();
            var fileContent = fileManager.ReadFile(filename);

            var result = Parser.MarkdownParser.Parse(fileContent);

            fileManager.WriteFile("result.html", result);
        }   

    }
}