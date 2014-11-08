using System;
using System.Collections;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MarkdownProcessor.Parser
{
    struct NodeInfo
    {
        public NodeType Type;
        public int ConsumedTextLength;
        public string InnerText;

        public NodeInfo(NodeType nodeType, int consumedTextLength, string innerText)
        {
            Type = nodeType;
            ConsumedTextLength = consumedTextLength;
            InnerText = innerText;
        }
    }

    class MarkdownParser
    {
        public static string Parse(string filecontent)
        {
            if (string.IsNullOrEmpty(filecontent)) return "";

            filecontent = NormalizeLineEndings(filecontent);

            var paragraphs = ParagraphExtractor.ExtractParagraphs(filecontent);

            var preprocessedParagraphs = paragraphs
                .Select(ParagraphPreprocessor.EscapeAngleBrackets)
                .Select(ParagraphPreprocessor.PreprocessParagraph)
                .ToArray();

            var parsedParagraphs = preprocessedParagraphs.Select(ParseParagraph);

            /*
                .Select(TagWrapper.Create(TagName.Code).Wrap)
                .Select(ParagraphPreprocessor.ReplaceUnderscoresInCodeToEntities)
                .Select(TagWrapper.Create(TagName.Em).Wrap)
                .Select(TagWrapper.Create(TagName.Strong).Wrap)
                .Select(TagWrapper.Create(TagName.Root).Wrap)
                .Select(ParagraphPreprocessor.PostprocessParagraph);

             */
            var postprocessedParagraphs = parsedParagraphs.Select(ParagraphPreprocessor.PostprocessParagraph);

            return string.Join("\n", postprocessedParagraphs);
        }

        public static string ParseParagraph(string paragraph)
        {
            var paragraphTree = BuildNode(NodeType.Root, paragraph);

            return paragraphTree.ToString();
        }

        private static ParagraphNode BuildNode(NodeType nodeType, string remainingText)
        {
            var node = new ParagraphNode(nodeType);
            if (nodeType == NodeType.Text)
            {
                node.SetText(remainingText);
                return node;
            }

            while (remainingText != "")
            {
                var currentNodeInfo = IdentifyCurrentNode(remainingText);
                remainingText = remainingText.Substring(currentNodeInfo.ConsumedTextLength);
                node.AddChild(BuildNode(currentNodeInfo.Type, currentNodeInfo.InnerText));
            }
            return node;
        }    

        private static string GetConsumedText(string innerText, NodeType type)
        {
            return Regex.Match(innerText, GetMarkdownPattern(type)).ToString();
        }

        public static NodeInfo IdentifyCurrentNode(string text)
        {
            foreach (var type in Enum.GetValues(typeof(NodeType)))
            {
                if ((NodeType)type == NodeType.Root) continue;
                var match = Regex.Match(text, GetMarkdownPattern(type));
                if (match.Success)
                    return new NodeInfo((NodeType)type, match.ToString().Length, match.Groups[1].ToString());
            }
            char currentMarker = text[0];
            var i = 1;
            while (i<text.Length && text[i] == currentMarker) i++;
            return new NodeInfo(NodeType.Text, i, new string(currentMarker,i));
        }

        private static string GetMarkdownPattern(object type)
        {
            return "^" + TagWrapper.Create((NodeType)type).MarkdownCapturePattern;
        }

        public static string NormalizeLineEndings(string text)
        {
            return
                new[] {"\r\n", "\n\r", "\r"}
                .Aggregate(text, (current, lineEnd) => current.Replace(lineEnd, "\n"));
        }
    }
}
