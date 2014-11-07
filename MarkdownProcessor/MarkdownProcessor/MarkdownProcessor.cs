using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MarkdownProcessor
{
    public class MarkdownProcessor
    {
        static void Main(string[] args)
        {
            IParameterManager parameterManager=new ConsoleParameterManager();
            var filename = parameterManager.GetFirstParameter(args);

            IFileManager fileManager = new FileManager();
            var fileContent = fileManager.ReadFile(filename);

            var result = ProcessAndGetHtml(fileContent);

            fileManager.WriteFile("result.html", result);
        }   

        public  static string ProcessAndGetHtml(string filecontent)
        {
            var paragraphs = ParagraphExtractor.ExtractParagraphs(filecontent);
            var processedParagraphs = paragraphs
                .Select(ParagraphPreprocessor.PreprocessParagraph)
                .Select(CodeWrapper.Wrap)
                .Select(ParagraphPreprocessor.ReplaceUnderscoresInCodeToEntities)
                .Select(WrapEm)
                .Select(WrapStrong)
                .Select(WrapP)
                .Select(ParagraphPreprocessor.PostprocessParagraph);

            return string.Join("\n", processedParagraphs);
        }

        public static string EscapeAngleBrackets(string textForEscape)
        {
            return textForEscape.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        public static string WrapEm(string input)
        {
            return Regex.Replace(input,
                @"(?<![\\_\w])_(" + @"(_{2,}|[^_])+" + @")(?<!\\)_(?![_\w])",
                "<em>$1</em>");
        }

        public static string WrapStrong(string input)
        {
            return Regex.Replace(input,
                @"(?<![\\_\w])__(?!_)(((?!__).)*)(?<![\\_])__(?![_\w])",
                "<strong>$1</strong>");
        }

        public static string UnescapeMarks(string textWithEscapedMarks)
        {
            return textWithEscapedMarks.Replace(@"\_", "_").Replace(@"\`", "`");
        }

        private static string WrapP(string input)
        {
            return "<p>" + input + "</p>";
        }

    }
}
