using NUnit.Framework;

namespace MarkdownProcessor.MarkdownProcessorTests
{
    [TestFixture]
    class MarkdownParserTestsForEm : MarkdownParserTests
    {
        [TestCase("Текст _окруженный с двух сторон_  одинарными символами подчерка превращается в тег em",
            "<p>Текст <em>окруженный с двух сторон</em>  одинарными символами подчерка превращается в тег em</p>")]
        [TestCase("Текст _окруженный \nсимволами_ подчеркивания в нескольких строках превращается в тег em",
            "<p>Текст <em>окруженный \nсимволами</em> подчеркивания в нескольких строках превращается в тег em</p>")]
        [TestCase("Внутри _выделения __ em ___ могут быть ____ любые _____ подчеркивания_, кроме одинарных.",
            "<p>Внутри <em>выделения __ em ___ могут быть ____ любые _____ подчеркивания</em>, кроме одинарных.</p>")]
        [TestCase("Внутри _выделения em __может быть__ strong_ выделение.",
            "<p>Внутри <em>выделения em <strong>может быть</strong> strong</em> выделение.</p>")]
        public void Parse_TextBetweenTwoUnderscores_ToEm(string input, string expected)
        {
            ParseCheck(input, expected);
        }

        [TestCase("Непарные подчеркивания: __Вот это_, не должно выделиться тегом em",
            "<p>Непарные подчеркивания: __Вот это_, не должно выделиться тегом em</p>")]
        [TestCase("Непарные подчеркивания: _Вот это__, не должно выделиться тегом em",
            "<p>Непарные подчеркивания: _Вот это__, не должно выделиться тегом em</p>")]
        [TestCase("Двойное подчеркивание __ не должено превращаться в пустой тег em",
            "<p>Двойное подчеркивание __ не должено превращаться в пустой тег em</p>")]
        [TestCase("В em могут превращаться ___только___ одинарные подчеркивания",
            "<p>В em могут превращаться ___только___ одинарные подчеркивания</p>")]
        public void Parse_NotSingleUnderscores_Ignore(string input, string expected)
        {
            ParseCheck(input, expected);
        }

        [Test]
        public void Parse_EscapedUnderscores_Ignore()
        {
            ParseCheck(@"Экранирование: \_Вот это\_, не должно выделиться тегом em",
                @"<p>Экранирование: \_Вот это\_, не должно выделиться тегом em</p>");
        }

        [Test]
        public void Parse_UnderscoresInTextAndDigits_Ignore()
        {
            ParseCheck("Подчерки_внутри_текста__и__цифр_12_3 не считаются выделением",
                "<p>Подчерки_внутри_текста__и__цифр_12_3 не считаются выделением</p>");
        }
    }
}
