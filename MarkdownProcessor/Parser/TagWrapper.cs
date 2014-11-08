using System;
using System.Text.RegularExpressions;

namespace MarkdownProcessor.Parser
{
    public enum TagName
    {
        Code,
        Em,
        Strong,
        P
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
                case TagName.P:
                    MarkdownCapturePattern = "(^.*$)";
                    ReplacementPattern = "<p>$1</p>";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("tagName");
            }
        }

        public string Wrap(string input)
        {
            return Regex.Replace(input, MarkdownCapturePattern, ReplacementPattern, RegexOptions.Singleline);
        }

        public string Wrap(string input, bool preprocess)
        {
            if (preprocess)
                return ParagraphPreprocessor.PostprocessParagraph
                    (Wrap(ParagraphPreprocessor.PreprocessParagraph(input)));
            return Wrap(input);
        }

        public static TagWrapper Create(TagName tagName)
        {
            return new TagWrapper(tagName);
        }
    }
}
