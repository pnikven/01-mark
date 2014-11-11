namespace MarkdownProcessor
{
    class TextNode : Node
    {
        string InnerText { get; set; }

        public TextNode(string text)
        {
            InnerText = text;
        }

        public override string ToString()
        {
            return InnerText;
        }

        public override bool CanContainOtherTags()
        {
            return false;
        }

        public static string GetPatternThatStartsFromTheBeginningOfString()
        {
            return "^([^`_]+)";
        }
    }
}