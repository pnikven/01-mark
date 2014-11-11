using System.Collections.Generic;
using System.Linq;

namespace MarkdownProcessor
{
    abstract class TagNode : Node
    {
        protected List<Node> Children = new List<Node>();

        public void AddChild(Node child)
        {
            Children.Add(child);
        }

        public override string ToString()
        {
            return WrapByTag(string.Join("", Children.Select(child => child.ToString())));
        }

        protected virtual string WrapByTag(string content)
        {
            return string.Format("<{0}>{1}</{0}>", GetTagName(), content);
        }

        protected abstract string GetTagName();
    }
}