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
            _patternToNodeMap=CreatePatternToNodeMap();
        }

        public string Parse(string content)
        {
            if (string.IsNullOrEmpty(content)) return "";

            content = NormalizeLineEndings(content);
            content = HttpUtility.HtmlEncode(content);

            var textConverter=new InternalRepresentationTextConverter();

            content = textConverter.Encode(content);
            var parsedContent = ParagraphExtractor.ExtractParagraphs(content)
                .Select(ParseParagraph)
                .Aggregate((p1,p2)=>p1+p2);
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
                foreach (var entry in _patternToNodeMap)
                {
                    var match = Regex.Match(remainingText, entry.Key);
                    if (!match.Success) continue;

                    remainingText = remainingText.Substring(match.ToString().Length);
                    var consumedText = match.Groups[1].ToString();
                    var nodeType = entry.Value;
                    var newNode = CreateNodeFromConsumedText(nodeType, consumedText);
                    node.AddChild(newNode);
                    if (nodeType.CanContainOtherTags) BuildNode((TagNode) newNode, consumedText);
                    break;
                }
            }
        }

        private static INode CreateNodeFromConsumedText(NodeType nodeType, string consumedText)
        {
            if(nodeType.Name=="text")return new TextNode(consumedText);
            if (nodeType.Name != "code") return new TagNode(nodeType.Name);
            var codeNode = new TagNode("code");
            codeNode.AddChild(new TextNode(consumedText));
            return codeNode;
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
