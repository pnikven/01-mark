using NUnit.Framework;
using System.Text.RegularExpressions;

namespace MarkdownProcessor
{
    class CodeWrapper
    {
        public static string Wrap(string input)
        {
            return Regex.Replace(input, "`([^`]+)`", "<code>$1</code>");
        }

        public static string Wrap(string input, bool preprocess)
        {
            if (preprocess)
                return ParagraphPreprocessor.PostprocessParagraph
                    (Wrap(ParagraphPreprocessor.PreprocessParagraph(input)));
            return Wrap(input);
        }
    }

    class CodeWrapperTests
    {
        [Test]
        public void WrapCode_TextBetweenBackticks_WrapByTagCode()
        {
            var input = @"Текст окруженный `одинарными _обратными_ кавычками` -> code";

            var result = CodeWrapper.Wrap(input, true);

            Assert.AreEqual(@"Текст окруженный <code>одинарными _обратными_ кавычками</code> -> code", result);
        }

        [Test]
        public void WrapCode_TextBetweenTwoEscapedBackticks_NotWrapToCode()
        {
            var input = @"Экранирование: \`Вот это\`, не должно выделиться тегом code";

            var result = CodeWrapper.Wrap(input, true);

            Assert.AreEqual(@"Экранирование: \`Вот это\`, не должно выделиться тегом code", result);
        }

        [TestCase("Текст окруженный ``несколькими _обратными_ кавычками`` ->X code",
            "Текст окруженный ``несколькими _обратными_ кавычками`` ->X code")]
        [TestCase("Текст с `` двойными _обратными_ кавычками ->X code",
            "Текст с `` двойными _обратными_ кавычками ->X code")]
        [TestCase("Текст с ``двойной и одинарной обратными кавычками` ->X code",
            "Текст с ``двойной и одинарной обратными кавычками` ->X code")]
        [TestCase("Текст с `одинарной и двойной обратными кавычками`` ->X code",
            "Текст с `одинарной и двойной обратными кавычками`` ->X code")]
        [TestCase("Текст с `окруженный обратными кавычками с повторяющимися обратными ``` кавычками` внутри",
            "Текст с <code>окруженный обратными кавычками с повторяющимися обратными ``` кавычками</code> внутри")]
        public void WrapCode_MutliBackticks_NotRecognizeAsMarks(string input, string expected)
        {
            var result = CodeWrapper.Wrap(input, true);

            Assert.AreEqual(expected, result);
        }
        
    }
}
