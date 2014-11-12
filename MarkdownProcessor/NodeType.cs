namespace MarkdownProcessor
{
    class NodeType
    {
        public string Name { get; private set; }
        public bool CanContainOtherTags{get; private set; }

        public NodeType(string name, bool canContainOtherTags)
        {
            CanContainOtherTags = canContainOtherTags;
            Name = name;
        }
    }
}