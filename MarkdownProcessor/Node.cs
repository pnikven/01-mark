namespace MarkdownProcessor
{
    abstract class Node
    {
        new abstract public string ToString();
        public abstract bool CanContainOtherTags();
    }
}
