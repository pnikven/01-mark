using System;
using System.Collections.Generic;
using System.Linq;

namespace MarkdownProcessor.Parser
{
    public enum NodeType
    {
        P,
        Code,
        Em,
        Strong,
        Text
    }

    public class TagMapper
    {
        private const string UnderscoreReplacement = "<US>";
        private const string BacktickReplacement = "<BT>";

        public static readonly Dictionary<string, string> ReplacementForMark =
            new Dictionary<string, string>()
            {
                    {"_", UnderscoreReplacement},
                    {"`", BacktickReplacement}
            };

        public static string AllMarkers = string.Join("", ReplacementForMark.Keys.ToArray());

        public static string GetMarkdownCapturePattern(NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.Code:
                    return "^`([^`]+)`(?!`)";
                case NodeType.Em:
                    return "^_((?:[^_]|[^_]+__+[^_]+)+?)_(?!_)";
                case NodeType.Strong:
                    return "^__((?:[^_]|[^_]+_+[^_]+)+?)__(?!_)";
                case NodeType.P:
                    throw new ArgumentException("MarkdownCapturePattern is not supported for this nodeType");
                case NodeType.Text:
                    return "^([^"+AllMarkers+"]+)";
                default:
                    throw new ArgumentOutOfRangeException("nodeType");
            }
        }
    }
}
