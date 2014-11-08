namespace MarkdownProcessor.Parser
{
    struct NodeInfo
    {
        public NodeType Type;
        public int ConsumedTextLength;
        public string InnerText;

        public NodeInfo(NodeType nodeType, int consumedTextLength, string innerText)
        {
            Type = nodeType;
            ConsumedTextLength = consumedTextLength;
            InnerText = innerText;
        }
    }
}