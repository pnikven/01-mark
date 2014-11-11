using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace MarkdownProcessor
{
    abstract class Node
    {
        public const string PatternNotSupported = "";
        new abstract public string ToString();
        protected abstract string GetPatternFromTheBeginningOfString();
    }

    class TextNode : Node
    {
        string InnerText { get; set; }

        public void SetText(string text)
        {
            InnerText = text;
        }

        public override string ToString()
        {
            return InnerText;
        }

        protected override string GetPatternFromTheBeginningOfString()
        {
            return "^([^`_]+)";
        }

        public static TextNode Create(string text)
        {
            var textNode=new TextNode();
            textNode.SetText(text);
            return textNode;
        }
    }

    abstract class TagNode : Node
    {
        protected static List<string> SupportedTags=new List<string>();
        protected List<Node> Children = new List<Node>();

        public void AddChild(Node child)
        {
            Children.Add(child);
        }

        public override string ToString()
        {
            return WrapByTag(string.Join("", Children.Select(child => child.ToString())));
        }

        private string WrapByTag(string content)
        {
            return string.Format("<{0}>{1}</{0}>", GetTagName(), content);
        }

        protected abstract string GetTagName();
    }

    class ParagraphNode:TagNode
    {
        protected override string GetTagName()
        {
            return "p";
        }

        protected override string GetPatternFromTheBeginningOfString()
        {
            return PatternNotSupported;
        }
    }

    class CodeNode: TagNode
    {
        protected override string GetTagName()
        {
            return "code";
        }

        protected override string GetPatternFromTheBeginningOfString()
        {
            return "^`([^`]+)`(?!`)";
        }

        public static CodeNode Create(string content)
        {
            var codeNode = new CodeNode();
            codeNode.AddChild(TextNode.Create(content));
            return codeNode;
        }
    }

    class EmNode : TagNode
    {
        protected override string GetTagName()
        {
            return "em";
        }

        protected override string GetPatternFromTheBeginningOfString()
        {
            return "^_((?:[^_]|[^_]+__+[^_]+)+?)_(?!_)";
        }
    }

    class StrongNode : TagNode
    {
        protected override string GetTagName()
        {
            return "strong";
        }

        protected override string GetPatternFromTheBeginningOfString()
        {
            return "^__((?:[^_]|[^_]+_+[^_]+)+?)__(?!_)";
        }
    }

    class NodeOriginal
    {
        NodeType NodeType { get; set; }
        string InnerText { get; set; }
        private List<NodeOriginal> children;

        public NodeOriginal(NodeType nodeType)
        {
            NodeType = nodeType;
        }

        public void SetText(string text)
        {
            InnerText = text;
        }

        public void AddChild(NodeOriginal child)
        {
            if (children == null)
                children = new List<NodeOriginal>();
            children.Add(child);
        }

        public override string ToString()
        {
            if (NodeType == NodeType.Text)
                return InnerText;
            return WrapByTag(string.Join("", children.Select(child => child.ToString())));
        }

        public string WrapByTag(string content)
        {
            string tagName;
            switch (NodeType)
            {
                case NodeType.P:
                    tagName = "p";
                    break;
                case NodeType.Code:
                    tagName = "code";
                    break;
                case NodeType.Em:
                    tagName = "em";
                    break;
                case NodeType.Strong:
                    tagName = "strong";
                    break;
                case NodeType.Text:
                    throw new NotImplementedException("Code must not reach here.");
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return string.Format("<{0}>{1}</{0}>", tagName, content);
        }
    }
}
