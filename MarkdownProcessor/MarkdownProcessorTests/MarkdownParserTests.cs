using NUnit.Framework;

namespace MarkdownProcessor.MarkdownProcessorTests
{
    class MarkdownParserTests
    {
        protected void ParseCheck(string input, string expected)
        {
            var markdownParser = new MarkdownParser();
            var result = markdownParser.Parse(input);
            Assert.AreEqual(expected, result);
        }
    }
}
