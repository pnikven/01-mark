using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MarkdownProcessor.MarkdownProcessorTests
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

        [Test]
        public void wrap_text_between_two_underscores_to_em()
        {
            var input = "Текст _окруженный с двух сторон_  одинарными символами подчерка";

            var result = emWrapper.Wrap(input, true);

            Assert.AreEqual("Текст <em>окруженный с двух сторон</em>  одинарными символами подчерка", result);
        }

        [Test]
        public void wrap_text_on_two_lines_between_two_underscores_to_em()
        {
            var input = "Текст _окруженный \nсимволами_ подчеркивания в нескольких строках";

            var result = emWrapper.Wrap(input, true);

            Assert.AreEqual("Текст <em>окруженный \nсимволами</em> подчеркивания в нескольких строках", result);
        }

        [Test]
        public void wrap_text_between_two_underscores_with_inner_double_underscore_to_em()
        {
            var input = "Внутри _выделения em может быть __ двойное подчеркивание_.";

            var result = emWrapper.Wrap(input, true);

            Assert.AreEqual("Внутри <em>выделения em может быть __ двойное подчеркивание</em>.", result);
        }

        [Test]
        public void wrap_text_between_two_underscores_with_inner_double_underscores_to_em()
        {
            var input = "Внутри _выделения em может быть __strong__ выделение_.";

            var result = emWrapper.Wrap(input, true);

            Assert.AreEqual("Внутри <em>выделения em может быть __strong__ выделение</em>.", result);
        }

        [Test]
        public void not_wrap_text_between_two_double_underscores_to_em()
        {
            var input = "Текст __окруженный с двух сторон__  двойными символами подчерка";

            var result = emWrapper.Wrap(input, true);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_two_escaped_underscores_to_em()
        {
            var input = @"Экранирование: \_Вот это\_, не должно выделиться тегом <em>";

            var result = emWrapper.Wrap(input, true);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_double_underscore_and_single_underscore_to_em()
        {
            var input = @"Экранирование: __Вот это_, не должно выделиться тегом <em>";

            var result = emWrapper.Wrap(input, true);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_single_underscore_and_double_underscore_to_em()
        {
            var input = @"Экранирование: _Вот это__, не должно выделиться тегом <em>";

            var result = emWrapper.Wrap(input, true);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_double_underscore_to_em()
        {
            var input = "Текст с двойным __ подчеркиванием";

            var result = emWrapper.Wrap(input, true);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_single_underscores_in_text_and_digits_to_em()
        {
            var input = "Подчерки_внутри_текста__и__цифр_12_3 не считаются выделением";

            var result = emWrapper.Wrap(input, true);

            Assert.AreEqual(input, result);
        }
    }
}
