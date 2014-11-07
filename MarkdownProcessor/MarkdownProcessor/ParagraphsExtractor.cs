using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MarkdownProcessor
{
    class ParagraphsExtractor
    {
        private const string ParagraphsRegexSplitter = @"\n\s*\n";

        public static string[] ExtractParagraphs(string markdown)
        {
            if (markdown == null)
                throw new ArgumentNullException("markdown text must be provided");
            return Regex.Split(markdown, ParagraphsRegexSplitter, RegexOptions.Singleline)
                .Where(st => st != "")
                .ToArray();
        }
    }
}
