using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MarkdownProcessor.Parser
{
    class ParagraphPreprocessor
    {
        private const string UnderscoreReplacement = "<US>";
        private const string BacktickReplacement = "<BT>";

        private static readonly Dictionary<string, string> ReplacementForMark = 
            new Dictionary<string, string>()
            {
                    {"_", UnderscoreReplacement},
                    {"`", BacktickReplacement}
            };

        public static string AllMarkers = string.Join("",ReplacementForMark.Keys.ToArray());

        public static string EscapeAngleBrackets(string textForEscape)
        {
            return textForEscape.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        public static string PreprocessParagraph(string paragraph)
        {
            var preprocessedParagraph = ReplaceEscapedMarksToEntities(paragraph);
            preprocessedParagraph = ReplaceUnderscoresInTextAndDigitsToEntities(preprocessedParagraph);
            preprocessedParagraph = ReplaceMultiBackticksToEntities(preprocessedParagraph);
            preprocessedParagraph = ReplaceMultiUnderscoresToEntities(preprocessedParagraph);
            return preprocessedParagraph;
        }

        public static string ReplaceEscapedMarksToEntities(string paragraph)
        {
            return ReplacementForMark
                    .Keys
                    .Aggregate(paragraph, (currentParagraph, escapedMark) =>
                        currentParagraph.Replace(@"\" + escapedMark, @"\" + ReplacementForMark[escapedMark]));
        }

        public static string ReplaceUnderscoresInTextAndDigitsToEntities(string paragraph)
        {
            return ReplaceInvalidMarksToEntities(paragraph,
                string.Format(@"[{0}]+_+[{0}]+(_+[{0}]+)*", @"\p{L}\p{Nd}"),
                "_");
        }

        public static string ReplaceMultiBackticksToEntities(string paragraph)
        {
            return ReplaceInvalidMarksToEntities(paragraph, "``+", "`");
        }

        public static string ReplaceMultiUnderscoresToEntities(string paragraph)
        {
            return ReplaceInvalidMarksToEntities(paragraph, "___+", "_");
        }

        public static string ReplaceUnderscoresInCodeToEntities(string paragraph)
        {
            return ReplaceInvalidMarksToEntities(paragraph, "<code>.*?_+.*?</code>", "_");
        }

        private static string ReplaceInvalidMarksToEntities(string paragraph, string pattern, string mark)
        {
            var matches =
                from object match in Regex.Matches(paragraph,pattern) select match.ToString();
            return matches
                .OrderByDescending(match=>match.Length)
                .Aggregate(paragraph, (currentParagraph, match) =>
                    currentParagraph.Replace(match, match.Replace(mark, ReplacementForMark[mark])));
        }

        public static string PostprocessParagraph(string paragraph)
        {
            var result = ReplaceEntitiesToTextRepresentation(paragraph);
            return result;
        }

        public static string ReplaceEntitiesToTextRepresentation(string paragraph)
        {
            return ReplacementForMark
                    .Values
                    .Aggregate(paragraph, (currentParagraph, entity) =>
                        currentParagraph.Replace(entity, 
                            ReplacementForMark.First(v=>v.Value==entity).Key));            
        }

    }
}
