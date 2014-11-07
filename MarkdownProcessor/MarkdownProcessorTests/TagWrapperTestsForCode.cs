using NUnit.Framework;

namespace MarkdownProcessor
{
    [TestFixture]
    class TagWrapperTestsForCode
    {
        private TagWrapper codeWrapper = null;

        [SetUp]
        public void Setup()
        {
            codeWrapper = new TagWrapper(TagName.Code);
        }

        [Test]
        public void Wrap_TextBetweenBackticks_WrapByTagCode()
        {
            var input = @"“екст окруженный `одинарными _обратными_ кавычками` -> code";

            var result = codeWrapper.Wrap(input, true);

            Assert.AreEqual(@"“екст окруженный <code>одинарными _обратными_ кавычками</code> -> code", result);
        }

        [Test]
        public void Wrap_TextBetweenTwoEscapedBackticks_NotWrapToCode()
        {
            var input = @"Ёкранирование: \`¬от это\`, не должно выделитьс€ тегом code";

            var result = codeWrapper.Wrap(input, true);

            Assert.AreEqual(@"Ёкранирование: \`¬от это\`, не должно выделитьс€ тегом code", result);
        }

        [TestCase("“екст окруженный ``несколькими _обратными_ кавычками`` ->X code",
            "“екст окруженный ``несколькими _обратными_ кавычками`` ->X code")]
        [TestCase("“екст с `` двойными _обратными_ кавычками ->X code",
            "“екст с `` двойными _обратными_ кавычками ->X code")]
        [TestCase("“екст с ``двойной и одинарной обратными кавычками` ->X code",
            "“екст с ``двойной и одинарной обратными кавычками` ->X code")]
        [TestCase("“екст с `одинарной и двойной обратными кавычками`` ->X code",
            "“екст с `одинарной и двойной обратными кавычками`` ->X code")]
        [TestCase("“екст с `окруженный обратными кавычками с повтор€ющимис€ обратными ``` кавычками` внутри",
            "“екст с <code>окруженный обратными кавычками с повтор€ющимис€ обратными ``` кавычками</code> внутри")]
        public void Wrap_MutliBackticks_NotRecognizeAsMarks(string input, string expected)
        {
            var result = codeWrapper.Wrap(input, true);

            Assert.AreEqual(expected, result);
        }
        
    }
}