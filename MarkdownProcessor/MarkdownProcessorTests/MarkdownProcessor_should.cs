using System;
using NUnit.Framework;

namespace MarkdownProcessor
{
    [TestFixture]
    public class MarkdownProcessor_should
    {


        [Test]
        public void escape_html_tags()
        {
            var input = "<p>One sentence.</p>";

            var result = MarkdownProcessor.EscapeAngleBrackets(input);

            Assert.AreEqual("&lt;p&gt;One sentence.&lt;/p&gt;", result);
        }

        [Test]
        public void wrap_two_sentences_to_two_paragraphs()
        {
            var input = "One sentence.\n    \nTwo sentence";

            var result = ParagraphExtractor.ExtractParagraphs(input);

            Assert.AreEqual(
                new[] {"One sentence.","Two sentence"},
                result);
        }

    }
}