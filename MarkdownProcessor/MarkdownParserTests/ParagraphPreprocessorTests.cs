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
            var input = @"Подчерки_внутри_текста__и__цифр_12_3 не _считаются_ выделением _и должны_ оставаться символами_подчерка.";

            var result = ParagraphPreprocessor.ReplaceUnderscoresInTextAndDigitsToEntities(input);

            Assert.AreEqual(@"Подчерки<US>внутри<US>текста<US><US>и<US><US>цифр<US>12<US>3 не _считаются_ выделением _и должны_ оставаться символами<US>подчерка.", result);
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
            var input = "Повторяющиеся цепочки обратных `` кавычек ``` заменяются на примитивы.";

            var result = ParagraphPreprocessor.ReplaceMultiBackticksToEntities(input);

            Assert.AreEqual("Повторяющиеся цепочки обратных <BT><BT> кавычек <BT><BT><BT> заменяются на примитивы.", result);
        }

        [Test]
        public void ReplaceMultiUnderscoresToEntities_MultiUnderscores_ReplacesToEntities()
        {
            var input = "Повторяющиеся __цепочки___ более _двух __ подчерков ___заменяются ____на примитивы.";

            var result = ParagraphPreprocessor.ReplaceMultiUnderscoresToEntities(input);

            Assert.AreEqual("Повторяющиеся __цепочки<US><US><US> более _двух __ подчерков <US><US><US>заменяются <US><US><US><US>на примитивы.", result);
        }
    }
}