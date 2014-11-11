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
        const BindingFlags BindFlags = BindingFlags.DeclaredOnly |
                                          BindingFlags.NonPublic |
                                          BindingFlags.Instance; 

        private readonly Hashtable _patternToNodeTypeMap;

        public MarkdownParser()
        {
            _patternToNodeTypeMap=CreatePatternToNodeHashmap();
        }

        public string Parse(string filecontent)
        {
            if (string.IsNullOrEmpty(filecontent)) return "";

            filecontent = NormalizeLineEndings(filecontent);

            filecontent = HttpUtility.HtmlEncode(filecontent);

            var textConverter=new InternalRepresentationTextConverter();

            filecontent = textConverter.Encode(filecontent);

            var paragraphs = ParagraphExtractor.ExtractParagraphs(filecontent);

            var parsedParagraphs = paragraphs.Select(ParseParagraph);

            var parsedContent = string.Join("\n", parsedParagraphs);

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
                foreach (var pattern in _patternToNodeTypeMap.Keys)
                {
                    var match = Regex.Match(remainingText, (string) pattern);
                    if (!match.Success) continue;

                    remainingText = remainingText.Substring(match.ToString().Length);
                    var consumedText = match.Groups[1].ToString();

                    var nodeType = (Type) _patternToNodeTypeMap[pattern];
                    if (nodeType == typeof(TextNode))
                    {
                        var textNode = (TextNode)nodeType.GetMethod("Create")
                            .Invoke(null, new object[] { consumedText });
                        node.AddChild(textNode);
                    }
                    else if (nodeType == typeof(CodeNode))
                    {
                        var codeNode = (CodeNode)nodeType.GetMethod("Create")
                            .Invoke(null, new object[] { consumedText });
                        node.AddChild(codeNode);
                    }
                    else
                    {
                        var tagNode = nodeType.GetConstructor(Type.EmptyTypes).Invoke(null);
                        node.AddChild((Node)tagNode);
                        BuildNode((TagNode)tagNode, consumedText);
                    }
                    if (remainingText == "") return;
                }
                if (remainingText == "") return;
                char currentMarker = remainingText[0];
                var i = 1;
                while (i < remainingText.Length && remainingText[i] == currentMarker) i++;
                node.AddChild(TextNode.Create(new string(currentMarker, i)));
                remainingText = remainingText.Substring(i);
            }
        }

        private Hashtable CreatePatternToNodeHashmap()
        {
            var patternToNodeTypeMap = new Hashtable();
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Type[] types = executingAssembly.GetTypes()
                .Where(type => type.IsSubclassOf(typeof (Node))).ToArray();
            foreach (var type in types)
            {
                var methodNames = type.GetMethods(BindFlags)
                    .Select(method => method.Name).ToArray();
                if (methodNames.Contains("GetPatternFromTheBeginningOfString"))
                {
                    var nodeInstance = CreateInstance(type);
                    var method = type.GetMethod("GetPatternFromTheBeginningOfString",
                            BindFlags);
                    var pattern = (string)method.Invoke(nodeInstance, null);
                    if (pattern != Node.PatternNotSupported)
                        patternToNodeTypeMap.Add(pattern, type);
                }
            }
            return patternToNodeTypeMap;
        }

        private static object CreateInstance(Type type)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var instance = Assembly.GetExecutingAssembly().CreateInstance(
                executingAssembly.GetName().Name + "." + type.Name);
            return instance;
        }
    }
}
