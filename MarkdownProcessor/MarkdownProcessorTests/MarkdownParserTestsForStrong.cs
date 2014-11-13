using NUnit.Framework;

namespace MarkdownProcessor.MarkdownProcessorTests
{
    [TestFixture]
    class MarkdownParserTestsForStrong : MarkdownParserTests
    {
        [TestCase("Текст с __двумя символами__ — д.б. жирным",
            "<p>Текст с <strong>двумя символами</strong> — д.б. жирным</p>")]
        [TestCase("Текст __окруженный \nсимволами__ двойного подчеркивания в нескольких строках д.б. жирным",
            "<p>Текст <strong>окруженный \nсимволами</strong> двойного подчеркивания в нескольких строках д.б. жирным</p>")]
        [TestCase("При трех __двойных __ подчеркиваниях __ в тег strong будут превращаться первые два",
            "<p>При трех <strong>двойных </strong> подчеркиваниях __ в тег strong будут превращаться первые два</p>")]
        [TestCase("Текст __с __ четырьмя __ двойными__ подчеркиваниями",
            "<p>Текст <strong>с </strong> четырьмя <strong> двойными</strong> подчеркиваниями</p>")]
        [TestCase("Внутри _выделения em может быть __strong__ выделение_.",
            "<p>Внутри <em>выделения em может быть <strong>strong</strong> выделение</em>.</p>")]
        [TestCase("Внутри __выделения _ strong ___ могут быть ____ любые подчеркивания__, кроме двойных.",
            "<p>Внутри <strong>выделения _ strong ___ могут быть ____ любые подчеркивания</strong>, кроме двойных.</p>")]
        [TestCase("Внутри __выделения strong может быть _em_ выделение__.",
            "<p>Внутри <strong>выделения strong может быть <em>em</em> выделение</strong>.</p>")]
        [TestCase("__s_s__",
            "<p><strong>s_s</strong></p>")]
        [TestCase("__s___s__",
            "<p><strong>s___s</strong></p>")]
        public void Parse_TextBetweenTwoDoubleUnderscores_ToStrong(string input, string expected)
        {
            ParseCheck(input, expected);
        }

        [Test]
        public void Parse_EscapedUnderscores_Ignore()
        {
            ParseCheck(@"Экранирование: \__Вот это\__, не должно выделиться тегом strong, а превратится в em",
                @"<p>Экранирование: \_<em>Вот это\_</em>, не должно выделиться тегом strong, а превратится в em</p>");
        }

        [TestCase("Не двойные подчеркивания: __Вот это___, не должно выделиться тегом strong",
            "<p>Не двойные подчеркивания: __Вот это___, не должно выделиться тегом strong</p>")]
        [TestCase("Не двойные подчеркивания: ___Вот это__, не должно выделиться тегом strong",
            "<p>Не двойные подчеркивания: ___Вот это__, не должно выделиться тегом strong</p>")]
        public void Parse_NotDoubleUnderscores_Ignore(string input, string expected)
        {
            ParseCheck(input, expected);
        }

        [Test]
        public void Parse_UnderscoresInTextAndDigits_Ignore()
        {
            ParseCheck("Подчерки_внутри_текста__и__цифр_12_3 не считаются выделением",
                "<p>Подчерки_внутри_текста__и__цифр_12_3 не считаются выделением</p>");
        }
    }
}
