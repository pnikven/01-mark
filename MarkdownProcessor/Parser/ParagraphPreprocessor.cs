using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MarkdownProcessor.Parser
{
    class ParagraphPreprocessor
    {
        public static string EscapeAngleBrackets(string textForEscape)
        {
            return textForEscape.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        public static string PreprocessParagraph(string paragraph)
        {
            var p = ReplaceEscapedMarksToEntities(paragraph);

            p = ReplaceUnderscoresInTextAndDigitsToEntities(p);

            p = ReplaceMultiBackticksToEntities(p);

            p = ReplaceMultiUnderscoresToEntities(p);

            return p;
        }

        public static string ReplaceEscapedMarksToEntities(string paragraph)
        {
            return TagMapper.ReplacementForMark
                    .Keys
                    .Aggregate(paragraph, (currentParagraph, escapedMark) =>
                        currentParagraph.Replace(@"\" + escapedMark, @"\" + TagMapper.ReplacementForMark[escapedMark]));
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

        private static string ReplaceInvalidMarksToEntities(string paragraph, string pattern, string mark)
        {
            var matches =
                from object match in Regex.Matches(paragraph,pattern) select match.ToString();
            return matches
                .OrderByDescending(match=>match.Length)
                .Aggregate(paragraph, (currentParagraph, match) =>
                    currentParagraph.Replace(match, match.Replace(mark, TagMapper.ReplacementForMark[mark])));
        }

        public static string PostprocessParagraph(string paragraph)
        {
            return ReplaceEntitiesToTextRepresentation(paragraph);
        }

        public static string ReplaceEntitiesToTextRepresentation(string paragraph)
        {
            return TagMapper.ReplacementForMark
                    .Values
                    .Aggregate(paragraph, (currentParagraph, entity) =>
                        currentParagraph.Replace(entity, 
                            TagMapper.ReplacementForMark.First(v=>v.Value==entity).Key));            
        }

    }
}
