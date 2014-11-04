using System;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace MarkdownProcessor
{
    public class MarkdownProcessor
    {
        private const string POpen = "<p>";
        private const string PClose = "</p>";
        const string PSeparatorPattern = @"\n\s*\n";
        public const string PSeparator = "\n\n";

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("File name must eb provided");
                return;
            }
            
            var filename = args[0];


        }

        public static string WrapDoubleNewlinesToParagraphs(string input)
        {
            if(input == null) throw new ArgumentException("input string must be provided");

            var paragraphs = Regex.Split(input, PSeparatorPattern, RegexOptions.Singleline)
                .Where(st => st != "")
                .Select(st => st.StartsWith(POpen) ? st : POpen + st)
                .Select(st => st.EndsWith(PClose) ? st : st + PClose)
                .ToArray();

            if (paragraphs.Length == 0) return POpen + PClose;
            else
                return string.Join(PSeparator, paragraphs );
        }
    }

    [TestFixture]
    public class MarkdownProcessor_should
    {
        [Test]
        public void throw_argument_exception_on_null()
        {
            string input = null;

            var result = Assert.Throws<ArgumentException>(
                () => MarkdownProcessor.WrapDoubleNewlinesToParagraphs(input));

            StringAssert.Contains("input string must be provided", result.Message);
        }

        [Test]
        public void wrap_empty_sentence_to_empty_paragraph()
        {
            var input = "";

            var result = MarkdownProcessor.WrapDoubleNewlinesToParagraphs("");

            Assert.AreEqual("<p></p>", result);
        }

        [Test]
        public void wrap_one_sentence_to_paragraph()
        {
            var input = "One sentence.";

            var result = MarkdownProcessor.WrapDoubleNewlinesToParagraphs(input);

            Assert.AreEqual("<p>One sentence.</p>", result);
        }

        [Test]
        public void leave_one_paragraph_unchanged()
        {
            var input = "<p>One sentence.</p>";

            var result = MarkdownProcessor.WrapDoubleNewlinesToParagraphs(input);

            Assert.AreEqual("<p>One sentence.</p>", result);
        }

        [Test]
        public void wrap_two_sentences_to_two_paragraphs()
        {
            var input = "One sentence.\n    \nTwo sentence";

            var result = MarkdownProcessor.WrapDoubleNewlinesToParagraphs(input);

            Assert.AreEqual("<p>One sentence.</p>" + MarkdownProcessor.PSeparator + "<p>Two sentence</p>", result);
        }

        [Test]
        public void leave_two_paragraphs_unchanged()
        {
            var input = "<p>One sentence.</p>\n    \n<p>Two sentence</p>";

            var result = MarkdownProcessor.WrapDoubleNewlinesToParagraphs(input);

            Assert.AreEqual("<p>One sentence.</p>" + MarkdownProcessor.PSeparator + "<p>Two sentence</p>", 
                result);
        }

        [Test]
        public void wrap_sentence_and_paragraph_to_two_paragraphs()
        {
            var input = "<p>One sentence.</p>\n    \nTwo sentence";

            var result = MarkdownProcessor.WrapDoubleNewlinesToParagraphs(input);

            Assert.AreEqual("<p>One sentence.</p>" + MarkdownProcessor.PSeparator + "<p>Two sentence</p>", result);
        }


    }


}
