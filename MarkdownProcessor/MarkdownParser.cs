using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MarkdownProcessor
{
    class MarkdownParser
    {
        private readonly Dictionary<string, NodeType> _patternToNodeMap;

        public MarkdownParser()
        {
            _patternToNodeMap = CreatePatternToNodeMap();
        }

        public string Parse(string content)
        {
            if (string.IsNullOrEmpty(content)) return "";

            content = NormalizeLineEndings(content);
            content = HttpUtility.HtmlEncode(content);

            var textConverter = new InternalRepresentationTextConverter();

            content = textConverter.Encode(content);
            var parsedContent = ParagraphExtractor.ExtractParagraphs(content)
                .Select(ParseParagraph)
                .Aggregate((p1, p2) => p1 + p2);
            var result = textConverter.Decode(parsedContent);

            return result;
        }

        public static string NormalizeLineEndings(string text)
        {
            return
                new[] { "\r\n", "\n\r", "\r" }
                .Aggregate(text, (current, lineEnd) => current.Replace(lineEnd, "\n"));
        }

        public string ParseParagraph(string text)
        {
            var paragraph = new TagNode("p");
            BuildNode(paragraph, text);
            return paragraph.GetHtml();
        }

        private void BuildNode(TagNode node, string remainingText)
        {
            while (remainingText != "")
            {
                var bMatch = false;
                foreach (var entry in _patternToNodeMap)
                {
                    var match = Regex.Match(remainingText, entry.Key);
                    if (!match.Success) continue;
                    bMatch = true;
                    remainingText = remainingText.Substring(match.ToString().Length);
                    var consumedText = match.Groups[1].ToString();
                    var nodeType = entry.Value;
                    if (nodeType.Name == "text") { node.AddChild(new TextNode(consumedText)); break; }
                    var newNode = new TagNode(nodeType.Name);
                    node.AddChild(newNode);
                    if (nodeType.CanContainOtherTags) BuildNode(newNode, consumedText);
                    else newNode.AddChild(new TextNode(consumedText));
                    break;
                }
                if (!bMatch) throw new Exception(
                    "Start of text [" + remainingText + "] was not recognized by any of the pattern");
            }
        }

        private Dictionary<string, NodeType> CreatePatternToNodeMap()
        {
            return new Dictionary<string, NodeType>
            {
                {"^_((?:[^_]|[^_]+__+[^_]+)+?)_(?!_)", new NodeType("em", true)},
                {"^__((?:[^_]|[^_]+_+[^_]+)+?)__(?!_)", new NodeType("strong", true)},
                {"^`((?:[^`]|[^`]+``+[^`]+)+?)`(?!`)", new NodeType("code", false)},
                {"^([^`_]+|`+|_+)", new NodeType("text", false)}
            };
        }
    }
}
