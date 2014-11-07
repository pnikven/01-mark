using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework.Constraints;

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

            var codeWrapper = new TagWrapper(TagName.Code);
            var emWrapper = new TagWrapper(TagName.Em);
            var strongWrapper = new TagWrapper(TagName.Strong);

            var processedParagraphs = paragraphs
                .Select(ParagraphPreprocessor.PreprocessParagraph)
                .Select(codeWrapper.Wrap)
                .Select(ParagraphPreprocessor.ReplaceUnderscoresInCodeToEntities)
                .Select(emWrapper.Wrap)
                .Select(strongWrapper.Wrap)
                .Select(WrapP)
                .Select(ParagraphPreprocessor.PostprocessParagraph);

            return string.Join("\n", processedParagraphs);
        }

        public static string EscapeAngleBrackets(string textForEscape)
        {
            return textForEscape.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        private static string WrapP(string input)
        {
            return "<p>" + input + "</p>";
        }

    }
}
