using System;
using System.Collections;
using NUnit.Framework;

namespace MarkdownProcessor.MarkdownProcessorTests
{
    class ParagraphsExtractorTests
    {
        private void ExtractParagraphsCheck(string input, IEnumerable expected)
        {
            var result = ParagraphExtractor.ExtractParagraphs(input);
            Assert.AreEqual(expected, result);
        }

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
            ExtractParagraphsCheck("", new string[] { });
        }

        [TestCase("One paragraph.",
            new[] { "One paragraph." })]
        [TestCase("p1\n\np2",
            new[] { "p1", "p2" })]
        [TestCase("p1\n \t \np2\n \tstill p2\n \np3",
            new[] { "p1", "p2\n \tstill p2", "p3" })]
        public void ExtractParagraphs_OnSomeParagraphs_ReturnsCollectionWithTheseParagraphs(string input, IEnumerable expected)
        {
            ExtractParagraphsCheck(input, expected);
        }

    }
}
