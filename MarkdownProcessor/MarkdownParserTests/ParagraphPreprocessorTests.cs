using MarkdownProcessor.Parser;
using NUnit.Framework;

namespace MarkdownProcessor.MarkdownParserTests
{
    [TestFixture]
    class ParagraphPreprocessorTests
    {
        [Test]
        public void EscapeAngleBrackets_AngleBrackets_EscapesToHtmlEntities()
        {
            var input = "Sentence with <b>angle</b> brackets.";

            var result = ParagraphPreprocessor.EscapeAngleBrackets(input);

            Assert.AreEqual("Sentence with &lt;b&gt;angle&lt;/b&gt; brackets.", result);
        }

        [Test]
        public void ReplaceUnderscoresInTextAndDigitsToEntities_UnderscoresInTextAndDigits_ReplacesToEntities()
        {
            var input = @"��������_������_������__�__����_12_3 �� _���������_ ���������� _� ������_ ���������� ���������_��������.";

            var result = ParagraphPreprocessor.ReplaceUnderscoresInTextAndDigitsToEntities(input);

            Assert.AreEqual(@"��������<US>������<US>������<US><US>�<US><US>����<US>12<US>3 �� _���������_ ���������� _� ������_ ���������� ���������<US>��������.", result);
        }

        [Test]
        public void ReplaceEscapedMarksToEntities_EscapedMarks_ReplacesToEntities()
        {
            var input = @"Some \``text\\ \wit\_h escaped\__ marks.";

            var result = ParagraphPreprocessor.ReplaceEscapedMarksToEntities(input);

            Assert.AreEqual(@"Some \<BT>`text\\ \wit\<US>h escaped\<US>_ marks.", result);
        }

        [Test]
        public void ReplaceEntitiesToTextRepresentation_Entities_ReplacesToTextRepresentation()
        {
            var input = @"Some \<BT>`text\\ \wit\<US>h entities\<US>_ <US>in text.";

            var result = ParagraphPreprocessor.ReplaceEntitiesToTextRepresentation(input);

            Assert.AreEqual(@"Some \``text\\ \wit\_h entities\__ _in text.", result);
        }

        [Test]
        public void ReplaceMultiBackticksToEntities_MultiBackticks_ReplacesToEntities()
        {
            var input = "������������� ������� �������� `` ������� ``` ���������� �� ���������.";

            var result = ParagraphPreprocessor.ReplaceMultiBackticksToEntities(input);

            Assert.AreEqual("������������� ������� �������� <BT><BT> ������� <BT><BT><BT> ���������� �� ���������.", result);
        }

        [Test]
        public void ReplaceMultiUnderscoresToEntities_MultiUnderscores_ReplacesToEntities()
        {
            var input = "������������� __�������___ ����� _���� __ ��������� ___���������� ____�� ���������.";

            var result = ParagraphPreprocessor.ReplaceMultiUnderscoresToEntities(input);

            Assert.AreEqual("������������� __�������<US><US><US> ����� _���� __ ��������� <US><US><US>���������� <US><US><US><US>�� ���������.", result);
        }
    }
}