using MarkdownProcessor.Parser;
using NUnit.Framework;

namespace MarkdownProcessor.MarkdownParserTests
{
    [TestFixture]
    class TagWrapperTestsForStrong
    {
        private TagWrapper strongWrapper = null;

        [SetUp]
        public void Setup()
        {
            strongWrapper = new TagWrapper(NodeType.Strong);
        }

        [TestCase("Текст с __двумя символами__ — д.б. жирным",
            "Текст с <strong>двумя символами</strong> — д.б. жирным")]
        [TestCase("Текст __окруженный \nсимволами__ двойного подчеркивания в нескольких строках",
            "Текст <strong>окруженный \nсимволами</strong> двойного подчеркивания в нескольких строках")]
        [TestCase("Текст с __тремя__ двойными__ подчеркиваниями",
            "Текст с <strong>тремя</strong> двойными__ подчеркиваниями")]
        [TestCase("Текст __с __ четырьмя __ двойными__ подчеркиваниями",
            "Текст <strong>с </strong> четырьмя <strong> двойными</strong> подчеркиваниями")]
        [TestCase("Внутри _выделения em может быть __strong__ выделение_.",
            "Внутри _выделения em может быть <strong>strong</strong> выделение_.")]
        [TestCase("Внутри __выделения strong ___ могут быть _ подчеркивания__, кроме двойных.",
            "Внутри <strong>выделения strong ___ могут быть _ подчеркивания</strong>, кроме двойных.")]
        [TestCase("Внутри __выделения strong может быть _em_ выделение__.",
            "Внутри <strong>выделения strong может быть _em_ выделение</strong>.")]
        [TestCase("__s_s__",
            "<strong>s_s</strong>")]
        [TestCase("__s___s__",
            "<strong>s___s</strong>")]
        public void Wrap_TextBetweenTwoDoubleUnderscores_ToStrong(string input, string expected)
        {
            var result = strongWrapper.Wrap(input, true);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Wrap_EscapedUnderscores_Ignore()
        {
            var input = @"Экранирование: \__Вот это\__, не должно выделиться тегом strong";

            var result = strongWrapper.Wrap(input, true);

            Assert.AreEqual(input, result);
        }

        [TestCase("Не двойные подчеркивания: __Вот это___, не должно выделиться тегом strong",
            "Не двойные подчеркивания: __Вот это___, не должно выделиться тегом strong")]
        [TestCase("Тройные подчеркивания: ___Вот это__, не должно выделиться тегом strong",
            "Тройные подчеркивания: ___Вот это__, не должно выделиться тегом strong")]
        public void Wrap_NotDoubleUnderscores_Ignore(string input, string expected)
        {
            var result = strongWrapper.Wrap(input, true);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Wrap_UnderscoresInTextAndDigits_Ignore()
        {
            var input = "Подчерки_внутри_текста__и__цифр_12_3 не считаются выделением";

            var result = strongWrapper.Wrap(input, true);

            Assert.AreEqual(input, result);
        }
    }
}
