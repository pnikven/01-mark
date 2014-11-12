using NUnit.Framework;

namespace MarkdownProcessor.MarkdownParserTests
{
    [TestFixture]
    class InternalRepresentationTextConverterTests
    {
        private InternalRepresentationTextConverter _internalRepresentationTextConverter;

        [SetUp]
        public void Setup()
        {
            _internalRepresentationTextConverter = new InternalRepresentationTextConverter();
        }

        [Test]
        public void Encode_UnderscoresInTextAndDigits_ReplacesToEntities()
        {
            var input = @"Подчерки_внутри_текста__и__цифр_12_3 не _считаются_ выделением _и должны_ оставаться символами_подчерка.";

            var result = _internalRepresentationTextConverter.Encode(input);

            Assert.AreEqual(@"Подчерки<US>внутри<US>текста<US><US>и<US><US>цифр<US>12<US>3 не _считаются_ выделением _и должны_ оставаться символами<US>подчерка.", result);
        }

        [Test]
        public void Encode_EscapedMarks_ReplacesToEntities()
        {
            var input = @"Some \``text\\ \wit\_h escaped\__ marks.";

            var result = _internalRepresentationTextConverter.Encode(input);

            Assert.AreEqual(@"Some \<BT>`text\\ \wit\<US>h escaped\<US>_ marks.", result);
        }

        [Test]
        public void Encode_ExtraBackticks_ReplacesToEntities()
        {
            var input = "Повторяющиеся цепочки обратных `` кавычек ``` заменяются на примитивы.";

            var result = _internalRepresentationTextConverter.Encode(input);

            Assert.AreEqual("Повторяющиеся цепочки обратных <BT><BT> кавычек <BT><BT><BT> заменяются на примитивы.", result);
        }

        [Test]
        public void Encode_ExtraUnderscores_ReplacesToEntities()
        {
            var input = "Повторяющиеся __цепочки___ более _двух __ подчерков ___заменяются ____на примитивы.";

            var result = _internalRepresentationTextConverter.Encode(input);

            Assert.AreEqual("Повторяющиеся __цепочки<US><US><US> более _двух __ подчерков <US><US><US>заменяются <US><US><US><US>на примитивы.", result);
        }

        [Test]
        public void Decode_Entities_ReplacesToTextRepresentation()
        {
            var input = @"Some \<BT>`text\\ \wit\<US>h entities\<US>_ <US>in text.";

            var result = _internalRepresentationTextConverter.Decode(input);

            Assert.AreEqual(@"Some \``text\\ \wit\_h entities\__ _in text.", result);
        }
    }
}