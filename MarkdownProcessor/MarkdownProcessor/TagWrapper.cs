using System;
using System.Text.RegularExpressions;

namespace MarkdownProcessor
{
    public enum TagName
    {
        Code,
        Em,
        Strong
    }

    public class TagWrapper
    {
        private string MarkdownCapturePattern { get; set; }
        private string ReplacementPattern { get; set; }

        public TagWrapper(TagName tagName)
        {
            switch (tagName)
            {
                case TagName.Code:
                    MarkdownCapturePattern = "`([^`]+)`";
                    ReplacementPattern = "<code>$1</code>";
                    break;
                case TagName.Em:
                    MarkdownCapturePattern = "(?<!_)_((?:[^_]|[^_]+__+[^_]+)+?)_(?!_)";
                    ReplacementPattern = "<em>$1</em>";
                    break;
                case TagName.Strong:
                    MarkdownCapturePattern = "(?<!_)__((?:[^_]|[^_]+_+[^_]+)+?)__(?!_)";
                    ReplacementPattern = "<strong>$1</strong>";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("tagName");
            }
        }

        public string Wrap(string input)
        {
            return Regex.Replace(input, MarkdownCapturePattern, ReplacementPattern);
        }

        public string Wrap(string input, bool preprocess)
        {
            if (preprocess)
                return ParagraphPreprocessor.PostprocessParagraph
                    (Wrap(ParagraphPreprocessor.PreprocessParagraph(input)));
            return Wrap(input);
        }
    }
}
