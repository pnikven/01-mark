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
            var input = @"����� ���������� `���������� _���������_ ���������` -> code";

            var result = codeWrapper.Wrap(input, true);

            Assert.AreEqual(@"����� ���������� <code>���������� _���������_ ���������</code> -> code", result);
        }

        [Test]
        public void Wrap_TextBetweenTwoEscapedBackticks_NotWrapToCode()
        {
            var input = @"�������������: \`��� ���\`, �� ������ ���������� ����� code";

            var result = codeWrapper.Wrap(input, true);

            Assert.AreEqual(@"�������������: \`��� ���\`, �� ������ ���������� ����� code", result);
        }

        [TestCase("����� ���������� ``����������� _���������_ ���������`` ->X code",
            "����� ���������� ``����������� _���������_ ���������`` ->X code")]
        [TestCase("����� � `` �������� _���������_ ��������� ->X code",
            "����� � `` �������� _���������_ ��������� ->X code")]
        [TestCase("����� � ``������� � ��������� ��������� ���������` ->X code",
            "����� � ``������� � ��������� ��������� ���������` ->X code")]
        [TestCase("����� � `��������� � ������� ��������� ���������`` ->X code",
            "����� � `��������� � ������� ��������� ���������`` ->X code")]
        [TestCase("����� � `���������� ��������� ��������� � �������������� ��������� ``` ���������` ������",
            "����� � <code>���������� ��������� ��������� � �������������� ��������� ``` ���������</code> ������")]
        public void Wrap_MutliBackticks_NotRecognizeAsMarks(string input, string expected)
        {
            var result = codeWrapper.Wrap(input, true);

            Assert.AreEqual(expected, result);
        }
        
    }
}