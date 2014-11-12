using System.Collections.Generic;
using System.Linq;

namespace MarkdownProcessor
{
    class TagNode : INode
    {
        private string TagName { get; set; }

        protected List<INode> Children = new List<INode>();

        public TagNode(string tagName)
        {
            TagName = tagName;
        }

        public void AddChild(INode child)
        {
            Children.Add(child);
        }

        public string GetHtml()
        {
            return WrapByTag(string.Join("", Children.Select(child => child.GetHtml())));
        }

        private string WrapByTag(string content)
        {
            return string.Format("<{0}>{1}</{0}>", GetTagName(), content);
        }

        string GetTagName()
        {
            return TagName;
        }
    }
}