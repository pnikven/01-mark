using System;
using System.Text.RegularExpressions;

namespace MarkdownProcessor.Parser
{
    public class TagWrapper
    {
        public string MarkdownCapturePattern { get; set; }
        private string ReplacementPattern { get; set; }

        public TagWrapper(NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.Code:
                    MarkdownCapturePattern = "`([^`]+)`";
                    ReplacementPattern = "<code>$1</code>";
                    break;
                case NodeType.Em:
                    MarkdownCapturePattern = "(?<!_)_((?:[^_]|[^_]+__+[^_]+)+?)_(?!_)";
                    ReplacementPattern = "<em>$1</em>";
                    break;
                case NodeType.Strong:
                    MarkdownCapturePattern = "(?<!_)__((?:[^_]|[^_]+_+[^_]+)+?)__(?!_)";
                    ReplacementPattern = "<strong>$1</strong>";
                    break;
                case NodeType.Root:
                    MarkdownCapturePattern = "(^.*$)";
                    ReplacementPattern = "<p>$1</p>";
                    break;
                case NodeType.Text:
                    MarkdownCapturePattern = "([^"+ParagraphPreprocessor.AllMarkers+"]+)";
                    ReplacementPattern = "$1";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("nodeType");
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

        public static TagWrapper Create(NodeType tagType)
        {
            return new TagWrapper(tagType);
        }
    }
}
