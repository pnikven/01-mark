using System;
using MarkdownProcessor.Parser;
using NUnit.Framework;

namespace MarkdownProcessor.MarkdownParserTests
{
    class ParagraphsExtractorTests
    {
        [Test]
        public void ExtractParagraphs_OnNull_ThrowsException()
        {
            var result = Assert.Throws<ArgumentNullException>(
                () => ParagraphExtractor.ExtractParagraphs(null));

            StringAssert.Contains("markdown text must be provided", result.Message);
        }

        [Test]
        public void ExtractParagraphs_OnEmpty_ReturnsEmptyCollection()
        {
            var input = "";

            var result = ParagraphExtractor.ExtractParagraphs(input);

            Assert.AreEqual(new string[] {  }, result);
        }

        [Test]
        public void ExtractParagraphs_OnOneSentence_ReturnsCollectionWithOneString()
        {
            var input = "One sentence.";

            var result = ParagraphExtractor.ExtractParagraphs(input);

            Assert.AreEqual(new[] { "One sentence." }, result);
        }
    }
}
