using MarkdownProcessor.Parser;
using NUnit.Framework;

namespace MarkdownProcessor.MarkdownParserTests
{
    [TestFixture]
    class MarkdownParserTestsForCode
    {
        void ParseCheck(string input, string expected)
        {
            var result = MarkdownParser.Parse(input);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Parse_TextBetweenBackticks_ToCode()
        {
            ParseCheck("Текст окруженный `одинарными _обратными_ кавычками` переходит в code",
                "<p>Текст окруженный <code>одинарными _обратными_ кавычками</code> переходит в code</p>");
        }

        [Test]
        public void Parse_EscapedBackticks_Ignore()
        {
            ParseCheck(@"Экранирование: \`Вот это\`, не должно выделиться тегом code",
                @"<p>Экранирование: \`Вот это\`, не должно выделиться тегом code</p>");
        }

        [TestCase("Текст окруженный ``несколькими _обратными_ кавычками`` не переходит в code",
            "<p>Текст окруженный ``несколькими <em>обратными</em> кавычками`` не переходит в code</p>")]
        [TestCase("Текст с `` двойными _обратными_ кавычками не переходит в code",
            "<p>Текст с `` двойными <em>обратными</em> кавычками не переходит в code</p>")]
        [TestCase("Текст с ``двойной и одинарной обратными кавычками` не переходит в code",
            "<p>Текст с ``двойной и одинарной обратными кавычками` не переходит в code</p>")]
        [TestCase("Текст с `одинарной и двойной обратными кавычками`` не переходит в code",
            "<p>Текст с `одинарной и двойной обратными кавычками`` не переходит в code</p>")]
        [TestCase("Текст с `окруженный обратными кавычками с повторяющимися обратными ``` кавычками` внутри",
            "<p>Текст с <code>окруженный обратными кавычками с повторяющимися обратными ``` кавычками</code> внутри</p>")]
        public void Parse_NotSingleBackticks_Ignore(string input, string expected)
        {
            ParseCheck(input,expected);
        }
        
    }
}