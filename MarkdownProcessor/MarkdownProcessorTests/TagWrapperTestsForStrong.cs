using NUnit.Framework;

namespace MarkdownProcessor.MarkdownProcessorTests
{
    [TestFixture]
    class TagWrapperTestsForStrong
    {
        private TagWrapper strongWrapper = null;

        [SetUp]
        public void Setup()
        {
            strongWrapper = new TagWrapper(TagName.Strong);
        }

        [Test]
        public void wrap_text_between_two_double_underscores_to_strong()
        {
            var input = "Текст с __двумя символами__ — д.б. жирным";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual("Текст с <strong>двумя символами</strong> — д.б. жирным", result);
        }

        [Test]
        public void wrap_text_between_first_two_double_underscores_to_strong()
        {
            var input = "Текст с __тремя__ двойными__ подчеркиваниями";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual("Текст с <strong>тремя</strong> двойными__ подчеркиваниями", result);
        }

        [Test]
        public void wrap_text_between_first_and_second_two_double_underscores_to_strong()
        {
            var input = "Текст __с __ четырьмя __ двойными__ подчеркиваниями";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual("Текст <strong>с </strong> четырьмя <strong> двойными</strong> подчеркиваниями", result);
        }

        [Test]
        public void wrap_text_between_double_underscores_within_single_underscores_to_strong()
        {
            var input = "Внутри _выделения em может быть __strong__ выделение_.";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual("Внутри _выделения em может быть <strong>strong</strong> выделение_.", result);
        }

        [Test]
        public void wrap_text_between_double_underscores_with_inner_single_underscore_to_strong()
        {
            var input = "Внутри __выделения strong может быть _ одинарное__ подчеркивание.";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual("Внутри <strong>выделения strong может быть _ одинарное</strong> подчеркивание.", result);
        }

        [Test]
        public void wrap_text_between_double_underscores_with_inner_single_underscores_to_strong()
        {
            var input = "Внутри __выделения strong может быть _em_ выделение__.";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual("Внутри <strong>выделения strong может быть _em_ выделение</strong>.", result);
        }

        [Test]
        public void not_wrap_text_between_two_escaped_double_underscores_to_strong()
        {
            var input = @"Экранирование: \__Вот это\__, не должно выделиться тегом strong";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_double_underscore_and_triple_underscore_to_strong()
        {
            var input = @"Тройные подчеркивания: __Вот это___, не должно выделиться тегом strong";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_triple_underscore_and_double_underscore_to_strong()
        {
            var input = @"Тройные подчеркивания: ___Вот это__, не должно выделиться тегом strong";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_double_underscores_in_text_and_digits_to_strong()
        {
            var input = "Подчерки_внутри_текста__и__цифр_12_3 не считаются выделением";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void test_from_email()
        {
            var input = @"__s_s__";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual("<strong>s_s</strong>", result);
        }

        [Test]
        public void test_from_email2()
        {
            var input = @"__s___s__";

            var result = MarkdownProcessor.ProcessAndGetHtml(input);

            Assert.AreEqual("<p><strong>s___s</strong></p>", result);
        }
    }
}
