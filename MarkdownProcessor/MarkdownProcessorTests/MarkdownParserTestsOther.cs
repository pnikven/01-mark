using NUnit.Framework;

namespace MarkdownProcessor.MarkdownProcessorTests
{
    [TestFixture]
    class MarkdownParserTestsOther : MarkdownParserTests
    {
        [TestCase("Строка с _пересекающимися__ _em и__ strong",
            "<p>Строка с <em>пересекающимися__ </em>em и__ strong</p>")]
        [TestCase("Строка с __пересекающимися_ __em и_ strong",
            "<p>Строка с <strong>пересекающимися_ </strong>em и_ strong</p>")]
        [TestCase("Тест с `кодом`_, после которого сразу идет_ выделение",
            "<p>Тест с <code>кодом</code><em>, после которого сразу идет</em> выделение</p>")]
        [TestCase("Тест с _выделением_`, после которого сразу идет` код",
            "<p>Тест с <em>выделением</em><code>, после которого сразу идет</code> код</p>")]
        public void Parse_SampleText_ToCorrectHtml(string input, string expected)
        {
            ParseCheck(input, expected);
        }

        [Test]
        public void Parse_AngleBrackets_EscapesToHtmlEntities()
        {
            ParseCheck("Sentence with <b>angle</b> brackets.",
                "<p>Sentence with &lt;b&gt;angle&lt;/b&gt; brackets.</p>");
        }

        [Test]
        public void NormalizeLineEndings_DifferentLineEndings_ReplacesToUnified()
        {
            var input = "String1\r\nStrong2\rString3\n\rString4\n";

            var result = MarkdownParser.NormalizeLineEndings(input);

            Assert.AreEqual("String1\nStrong2\nString3\nString4\n", result);
        }
    }
}
