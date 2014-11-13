using NUnit.Framework;

namespace MarkdownProcessor.MarkdownProcessorTests
{
    [TestFixture]
    class InternalRepresentationTextConverterTests
    {
        private void EncodeCheck(string input, string expected)
        {
            var internalRepresentationTextConverter = new InternalRepresentationTextConverter();
            var result = internalRepresentationTextConverter.Encode(input);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Encode_UnderscoresInTextAndDigits_ReplacesToEntities()
        {
            EncodeCheck("Подчерки_внутри_текста__и__цифр_12_3 не _считаются_ выделением _и должны_ оставаться символами_подчерка.",
                "Подчерки<US>внутри<US>текста<US><US>и<US><US>цифр<US>12<US>3 не _считаются_ выделением _и должны_ оставаться символами<US>подчерка.");
        }

        [Test]
        public void Encode_EscapedMarks_ReplacesToEntities()
        {
            EncodeCheck(@"Some \``text\\ \wit\_h escaped\__ marks.",
                @"Some \<BT>`text\\ \wit\<US>h escaped\<US>_ marks.");
        }

        [Test]
        public void Encode_ExtraBackticks_ReplacesToEntities()
        {
            EncodeCheck("Повторяющиеся цепочки обратных `` кавычек ``` заменяются на примитивы.",
                "Повторяющиеся цепочки обратных <BT><BT> кавычек <BT><BT><BT> заменяются на примитивы.");
        }

        [Test]
        public void Encode_ExtraUnderscores_ReplacesToEntities()
        {
            EncodeCheck("Повторяющиеся __цепочки___ более _двух __ подчерков ___заменяются ____на примитивы.",
                "Повторяющиеся __цепочки<US><US><US> более _двух __ подчерков <US><US><US>заменяются <US><US><US><US>на примитивы.");
        }

        [Test]
        public void Decode_Entities_ReplacesToTextRepresentation()
        {
            var input = @"Some \<BT>`text\\ \wit\<US>h entities\<US>_ <US>in text.";

            var internalRepresentationTextConverter = new InternalRepresentationTextConverter();
            var result = internalRepresentationTextConverter.Decode(input);

            Assert.AreEqual(@"Some \``text\\ \wit\_h entities\__ _in text.", result);
        }
    }
}