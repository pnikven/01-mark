namespace MarkdownProcessor
{
    class CodeNode: TagNode
    {
        public CodeNode(string content)
        {
            AddChild(new TextNode(content));
        }

        protected override string GetTagName()
        {
            return "code";
        }

        public static string GetPatternThatStartsFromTheBeginningOfString()
        {
            return "^`([^`]+)`(?!`)";
        }

        public override bool CanContainOtherTags()
        {
            return false;
        }
    }
}