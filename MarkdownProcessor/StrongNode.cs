namespace MarkdownProcessor
{
    class StrongNode : TagNode
    {
        protected override string GetTagName()
        {
            return "strong";
        }

        public static string GetPatternThatStartsFromTheBeginningOfString()
        {
            return "^__((?:[^_]|[^_]+_+[^_]+)+?)__(?!_)";
        }

        public override bool CanContainOtherTags()
        {
            return true;
        }
    }
}