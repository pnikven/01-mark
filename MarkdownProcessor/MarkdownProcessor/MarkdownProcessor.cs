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

        private static string ProcessAndGetHtml(string filecontent)
        {
            return string.Join("\n",
                ParagraphsExtractor.ExtractParagraphs(filecontent)
                    .Select(p => 
                        WrapP(
                        UnescapeMarks(
                        WrapStrong(
                        WrapEm(
                        WrapCodeAndEscapeMarksInCode(
                        EscapeAngleBrackets(p))))))));
        }

        public static string EscapeAngleBrackets(string textForEscape)
        {
            return textForEscape.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        public static string WrapCodeAndEscapeMarksInCode(string input)
        {
            var codeWrapped = WrapCode(input);
            return EscapeMarksInCode(codeWrapped);
        }

        public static string WrapCode(string input)
        {
            return Regex.Replace(input,
                @"(?<![\\`])`(" + @"[^`]+" + @")(?<!\\)`(?!`)",
                "<code>$1</code>");
        }

        public static string EscapeMarksInCode(string textToEscape)
        {
            var escapeMarks = false;
            var result = Regex.Split(textToEscape, "(<code>|</code>)")
                .Select(st =>
                {
                    var resultString = escapeMarks ? st.Replace("_", "\\_") : st;
                    escapeMarks = st == "<code>";
                    return resultString;
                }).ToArray();
            return string.Join("", result);
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
