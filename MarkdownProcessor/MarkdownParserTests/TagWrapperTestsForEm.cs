using MarkdownProcessor.Parser;
using NUnit.Framework;

namespace MarkdownProcessor.MarkdownParserTests
{
    [TestFixture]
    class TagWrapperTestsForEm
    {
        private TagWrapper emWrapper = null ;

        [SetUp]
        public void Setup()
        {
            emWrapper=new TagWrapper(TagName.Em);
        }

        [TestCase("Текст _окруженный с двух сторон_  одинарными символами подчерка",
            "Текст <em>окруженный с двух сторон</em>  одинарными символами подчерка")]
        [TestCase("Текст _окруженный \nсимволами_ подчеркивания в нескольких строках",
            "Текст <em>окруженный \nсимволами</em> подчеркивания в нескольких строках")]
        [TestCase("Внутри _выделения em __ могут быть ___ подчеркивания_, кроме одинарных.",
            "Внутри <em>выделения em __ могут быть ___ подчеркивания</em>, кроме одинарных.")]
        [TestCase("Внутри _выделения em может быть __ двойное_ подчеркивание.",
            "Внутри <em>выделения em может быть __ двойное</em> подчеркивание.")]
        [TestCase("Внутри _выделения em __может быть__ strong_ выделение.",
            "Внутри <em>выделения em __может быть__ strong</em> выделение.")]
        [TestCase("",
            "")]
        public void Wrap_TextBetweenTwoUnderscores_ToEm(string input, string expected)
        {
            var result = emWrapper.Wrap(input, true);

            Assert.AreEqual(expected, result);
        }

        [TestCase("Текст __окруженный с двух сторон__  двойными символами подчерка",
            "Текст __окруженный с двух сторон__  двойными символами подчерка")]
        [TestCase("Экранирование: __Вот это_, не должно выделиться тегом <em>",
            "Экранирование: __Вот это_, не должно выделиться тегом <em>")]
        [TestCase("Экранирование: _Вот это__, не должно выделиться тегом <em>",
            "Экранирование: _Вот это__, не должно выделиться тегом <em>")]
        [TestCase("Текст с двойным __ подчеркиванием",
            "Текст с двойным __ подчеркиванием")]
        [TestCase("__s_s__",
            "__s_s__")]
        [TestCase("__s___s__",
            "__s___s__")]
        [TestCase("",
            "")]
        public void Wrap_NotSingleUnderscores_Ignore(string input, string expected)
        {
            var result = emWrapper.Wrap(input, true);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Wrap_EscapedUnderscores_Ignore()
        {
            var input = @"Экранирование: \_Вот это\_, не должно выделиться тегом <em>";

            var result = emWrapper.Wrap(input, true);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void Wrap_UnderscoresInTextAndDigits_Ignore()
        {
            var input = "Подчерки_внутри_текста__и__цифр_12_3 не считаются выделением";

            var result = emWrapper.Wrap(input, true);

            Assert.AreEqual(input, result);
        }
    }
}
