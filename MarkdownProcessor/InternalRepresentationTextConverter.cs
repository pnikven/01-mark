using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MarkdownProcessor
{
    class InternalRepresentationTextConverter
    {
        private readonly Dictionary<string, string> _replacementForMark;

        public InternalRepresentationTextConverter()
        {
            _replacementForMark = new Dictionary<string, string>()
            {
                {"_", "<US>"},
                {"`", "<BT>"}
            };
        }

        public string Encode(string text)
        {
            var p = ReplaceEscapedMarksToEntities(text);

            p = ReplaceUnderscoresInTextAndDigitsToEntities(p);

            p = ReplaceMarksInPatternToEntities(p, "``+", "`");
            p = ReplaceMarksInPatternToEntities(p, "___+", "_");

            return p;
        }

        private string ReplaceEscapedMarksToEntities(string text)
        {
            return _replacementForMark
                    .Aggregate(text, (currentText, replacement) =>
                        currentText.Replace(@"\" + replacement.Key, @"\" + replacement.Value));
        }

        private string ReplaceUnderscoresInTextAndDigitsToEntities(string text)
        {
            return ReplaceMarksInPatternToEntities(text,
                string.Format(@"[{0}]+_+[{0}]+(_+[{0}]+)*", @"\p{L}\p{Nd}"),
                "_");
        }

        private string ReplaceMarksInPatternToEntities(string text, string pattern, string mark)
        {
            return
                Regex.Replace(text, pattern, match => 
                    match.ToString().Replace(mark,_replacementForMark[mark]));
        }

        public string Decode(string text)
        {
            return ReplaceEntitiesToTextRepresentation(text);
        }

        private string ReplaceEntitiesToTextRepresentation(string encodedText)
        {
            return _replacementForMark
                    .Aggregate(encodedText, (currentText, replacement) =>
                        currentText.Replace(replacement.Value, replacement.Key));            
        }

    }
}
