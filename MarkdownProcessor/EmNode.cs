namespace MarkdownProcessor
{
    class EmNode : TagNode
    {
        protected override string GetTagName()
        {
            return "em";
        }

        public static string GetPatternThatStartsFromTheBeginningOfString()
        {
            return "^_((?:[^_]|[^_]+__+[^_]+)+?)_(?!_)";
        }

        public override bool CanContainOtherTags()
        {
            return true;
        }
    }
}