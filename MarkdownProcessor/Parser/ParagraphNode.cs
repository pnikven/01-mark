using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace MarkdownProcessor.Parser
{
    public enum NodeType
    {
        Root,
        Code,
        Em,
        Strong,
        Text
    }

    class ParagraphNode
    {
        NodeType ParagraphNodeType { get; set; }
        string InnerText { get; set; }
        private List<ParagraphNode> nodeChilds;

        public ParagraphNode(NodeType paragraphNodeType)
        {
            ParagraphNodeType = paragraphNodeType;
        }

        public void SetText(string text)
        {
            InnerText = text;
        }

        public void AddChild(ParagraphNode child)
        {
            if (nodeChilds == null)
                nodeChilds = new List<ParagraphNode>();
            nodeChilds.Add(child);
        }

        public override string ToString()
        {
            if (ParagraphNodeType == NodeType.Text)
                return InnerText;
            return WrapByTag(string.Join("", nodeChilds.Select(child => child.ToString())));
        }

        public string WrapByTag(string content)
        {
            string tagName;
            switch (ParagraphNodeType)
            {
                case NodeType.Root:
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
