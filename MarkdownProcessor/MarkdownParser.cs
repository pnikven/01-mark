using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace MarkdownProcessor
{
    class MarkdownParser
    {
        private readonly Hashtable _patternToNodeMap;

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
            var paragraph = new ParagraphNode();

            BuildNode(paragraph, text);

            return paragraph.ToString();
        }

        private void BuildNode(TagNode node, string remainingText)
        {
            while (remainingText != "")
            {
                bool bMatch = false;
                foreach (var pattern in _patternToNodeMap.Keys)
                {
                    var match = Regex.Match(remainingText, (string) pattern);
                    if (!match.Success) continue;

                    bMatch = true;
                    remainingText = remainingText.Substring(match.ToString().Length);
                    var consumedText = match.Groups[1].ToString();
                    var nodeType = (Type) _patternToNodeMap[pattern];
                    var newNode = CreateNodeFromConsumedText(nodeType, consumedText);
                    node.AddChild(newNode);
                    if (newNode.CanContainOtherTags()) BuildNode((TagNode)newNode, consumedText);
                    break;
                }

                if(!bMatch)
                    remainingText = AddSequenceOfFirstEqualCharsAsChildTextNodeIfNoPatternMatchs(node, remainingText);
            }
        }

        private static string AddSequenceOfFirstEqualCharsAsChildTextNodeIfNoPatternMatchs(TagNode node, string remainingText)
        {
            char firstChar = remainingText[0];
            var sequenceOfFirstEqualChars = Regex.Match(remainingText, "^" + firstChar + "+").ToString();
            node.AddChild(new TextNode(sequenceOfFirstEqualChars));
            remainingText = remainingText.Substring(sequenceOfFirstEqualChars.Length);
            return remainingText;
        }

        private static Node CreateNodeFromConsumedText(Type nodeType, string consumedText)
        {
            var constructorWithStringParameter = nodeType.GetConstructor(new[] {typeof (String)});
            if (constructorWithStringParameter != null)
            {
                return (Node) constructorWithStringParameter.Invoke(new object[] {consumedText});
            }

            var defaultConstructor = nodeType.GetConstructor(Type.EmptyTypes);
            if (defaultConstructor != null) return (Node) defaultConstructor.Invoke(null);

            throw new Exception("Default constructor for " + nodeType.Name + " is not available");
        }

        private Hashtable CreatePatternToNodeMap()
        {
            var patternToNodeMap = new Hashtable();
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Type[] nodeTypes = executingAssembly.GetTypes()
                .Where(type => type.IsSubclassOf(typeof (Node))).ToArray();
            foreach (var type in nodeTypes)
            {
                var method = type.GetMethod("GetPatternThatStartsFromTheBeginningOfString");
                if (method != null)
                {
                    var pattern = (string)method.Invoke(null, null);
                    patternToNodeMap.Add(pattern, type);                    
                }
            }
            return patternToNodeMap;
        }
    }
}
