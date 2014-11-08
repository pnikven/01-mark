using MarkdownProcessor.Parser;
using NUnit.Framework;

namespace MarkdownProcessor.MarkdownParserTests
{
    class MarkdownParserTestsForP
    {
        [TestCase("Предложение из одной строки.",
            "<p>Предложение из одной строки.</p>")]
        [TestCase("Предложение из нескольких строк.\nВторая строка\nТретья строка...",
            "<p>Предложение из нескольких строк.\nВторая строка\nТретья строка...</p>")]
        public void Parse_AnyString_ToP(string input, string expected)
        {
            var result = MarkdownParser.Parse(input);

            Assert.AreEqual(expected, result);
        }
    }
}
