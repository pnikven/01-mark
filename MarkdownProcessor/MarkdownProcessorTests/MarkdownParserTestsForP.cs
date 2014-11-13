using NUnit.Framework;

namespace MarkdownProcessor.MarkdownProcessorTests
{
    class MarkdownParserTestsForP : MarkdownParserTests
    {
        [TestCase("Предложение из одной строки.",
            "<p>Предложение из одной строки.</p>")]
        [TestCase("Предложение из нескольких строк.\nВторая строка\nТретья строка...",
            "<p>Предложение из нескольких строк.\nВторая строка\nТретья строка...</p>")]
        public void Parse_AnyString_ToP(string input, string expected)
        {
            ParseCheck(input, expected);
        }
    }
}
