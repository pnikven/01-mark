using MarkdownProcessor.Parser;
using NUnit.Framework;

namespace MarkdownProcessor.MarkdownParserTests
{
    [TestFixture]
    class MarkdownParserTests
    {
        [TestCase("Строка с _пересекающимися__ _em и__ strong",
            "<p>Строка с <em>пересекающимися<strong> </em>em и</strong> strong</p>")]
        [TestCase("Текст _окруженный с двух сторон_  одинарными символами подчерка",
            "<p>Текст <em>окруженный с двух сторон</em>  одинарными символами подчерка</p>")]
        [TestCase("Текст _окруженный \nсимволами_ подчеркивания в нескольких строках",
            "<p>Текст <em>окруженный \nсимволами</em> подчеркивания в нескольких строках</p>")]
        [TestCase("Внутри _выделения em __ могут быть ___ подчеркивания_, кроме одинарных.",
            "<p>Внутри <em>выделения em __ могут быть ___ подчеркивания</em>, кроме одинарных.</p>")]
        [TestCase("Внутри _выделения em может быть __ двойное_ подчеркивание.",
            "<p>Внутри <em>выделения em может быть __ двойное</em> подчеркивание.</p>")]
        [TestCase("Внутри _выделения em __может быть__ strong_ выделение.",
            "<p>Внутри <em>выделения em <strong>может быть</strong> strong</em> выделение.</p>")]
        [TestCase("",
            "")]
        public void Parse_SampleText_ToCorrectHtml(string input, string expected)
        {
            var result = MarkdownParser.Parse(input);

            Assert.AreEqual(expected, result);
        }
    }
}
