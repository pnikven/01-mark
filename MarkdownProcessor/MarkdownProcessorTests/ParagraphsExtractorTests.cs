using NUnit.Framework;
using System;

namespace MarkdownProcessor.MarkdownProcessorTests
{
    class ParagraphsExtractorTests
    {
        [Test]
        public void ExtractParagraphs_OnNull_ThrowsException()
        {
            var result = Assert.Throws<ArgumentNullException>(
                () => ParagraphsExtractor.ExtractParagraphs(null));

            StringAssert.Contains("markdown text must be provided", result.Message);
        }

        [Test]
        public void ExtractParagraphs_OnEmpty_ReturnsEmptyCollection()
        {
            var input = "";

            var result = ParagraphsExtractor.ExtractParagraphs(input);

            Assert.AreEqual(new string[] {  }, result);
        }

        [Test]
        public void ExtractParagraphs_OnOneSentence_ReturnsCollectionWithOneString()
        {
            var input = "One sentence.";

            var result = ParagraphsExtractor.ExtractParagraphs(input);

            Assert.AreEqual(new[] { "One sentence." }, result);
        }
    }
}
