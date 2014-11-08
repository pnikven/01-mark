using MarkdownProcessor.Parser;
using NUnit.Framework;

namespace MarkdownProcessor.MarkdownParserTests
{
    class TagWrapperTestsForP
    {
        private TagWrapper pWrapper = null;

        [SetUp]
        public void Setup()
        {
            pWrapper = new TagWrapper(TagName.P);
        }

        [TestCase("Предложение из одной строки.",
            "<p>Предложение из одной строки.</p>")]
        [TestCase("Предложение из нескольких строк.\nВторая строка\nТретья строка...",
            "<p>Предложение из нескольких строк.\nВторая строка\nТретья строка...</p>")]
        public void Wrap_AnyString_ToP(string input, string expected)
        {
            var result = pWrapper.Wrap(input, true);

            Assert.AreEqual(expected, result);
        }
    }
}
