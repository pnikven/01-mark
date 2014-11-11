namespace MarkdownProcessor
{
    class ParagraphNode:TagNode
    {
        protected override string GetTagName()
        {
            return "p";
        }

        public override bool CanContainOtherTags()
        {
            return true;
        }
    }
}