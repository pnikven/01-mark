namespace MarkdownProcessor
{
    class TextNode : INode
    {
        string InnerText { get; set; }

        public TextNode(string text)
        {
            InnerText = text;
        }

        public string GetHtml()
        {
            return InnerText;
        }
    }
}