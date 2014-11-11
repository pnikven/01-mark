using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MarkdownProcessor
{
    class InternalRepresentationTextConverter
    {
        private const string UnderscoreReplacement = "<US>";
        private const string BacktickReplacement = "<BT>";

        private readonly Dictionary<string, string> _replacementForMark;

        public InternalRepresentationTextConverter()
        {
            _replacementForMark = new Dictionary<string, string>()
            {
                {"_", UnderscoreReplacement},
                {"`", BacktickReplacement}
            };
        }

        public string Encode(string text)
        {
            var p = ReplaceEscapedMarksToEntities(text);

            p = ReplaceUnderscoresInTextAndDigitsToEntities(p);

            p = ReplaceMultiBackticksToEntities(p);

            p = ReplaceMultiUnderscoresToEntities(p);

            return p;
        }

        string ReplaceEscapedMarksToEntities(string text)
        {
            return _replacementForMark
                    .Keys
                    .Aggregate(text, (currentParagraph, escapedMark) =>
                        currentParagraph.Replace(@"\" + escapedMark, @"\" + _replacementForMark[escapedMark]));
        }

        string ReplaceUnderscoresInTextAndDigitsToEntities(string text)
        {
            return ReplaceMarksInPatternToEntities(text,
                string.Format(@"[{0}]+_+[{0}]+(_+[{0}]+)*", @"\p{L}\p{Nd}"),
                "_");
        }

        string ReplaceMultiBackticksToEntities(string text)
        {
            return ReplaceMarksInPatternToEntities(text, "``+", "`");
        }

        string ReplaceMultiUnderscoresToEntities(string text)
        {
            return ReplaceMarksInPatternToEntities(text, "___+", "_");
        }

        string ReplaceMarksInPatternToEntities(string text, string pattern, string mark)
        {
            var matches =
                from object match in Regex.Matches(text,pattern) select match.ToString();
            return matches
                .OrderByDescending(match=>match.Length)
                .Aggregate(text, (currentParagraph, match) =>
                    currentParagraph.Replace(match, match.Replace(mark, _replacementForMark[mark])));
        }

        public string Decode(string text)
        {
            return ReplaceEntitiesToTextRepresentation(text);
        }

        string ReplaceEntitiesToTextRepresentation(string encodedText)
        {
            return _replacementForMark
                    .Values
                    .Aggregate(encodedText, (currentParagraph, entity) =>
                        currentParagraph.Replace(entity, 
                            _replacementForMark.First(v=>v.Value==entity).Key));            
        }

    }
}
