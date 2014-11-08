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
            ParseCheck("����� ���������� `���������� _���������_ ���������` ��������� � code",
                "<p>����� ���������� <code>���������� _���������_ ���������</code> ��������� � code</p>");
        }

        [Test]
        public void Parse_EscapedBackticks_Ignore()
        {
            ParseCheck(@"�������������: \`��� ���\`, �� ������ ���������� ����� code",
                @"<p>�������������: \`��� ���\`, �� ������ ���������� ����� code</p>");
        }

        [TestCase("����� ���������� ``����������� _���������_ ���������`` �� ��������� � code",
            "<p>����� ���������� ``����������� <em>���������</em> ���������`` �� ��������� � code</p>")]
        [TestCase("����� � `` �������� _���������_ ��������� �� ��������� � code",
            "<p>����� � `` �������� <em>���������</em> ��������� �� ��������� � code</p>")]
        [TestCase("����� � ``������� � ��������� ��������� ���������` �� ��������� � code",
            "<p>����� � ``������� � ��������� ��������� ���������` �� ��������� � code</p>")]
        [TestCase("����� � `��������� � ������� ��������� ���������`` �� ��������� � code",
            "<p>����� � `��������� � ������� ��������� ���������`` �� ��������� � code</p>")]
        [TestCase("����� � `���������� ��������� ��������� � �������������� ��������� ``` ���������` ������",
            "<p>����� � <code>���������� ��������� ��������� � �������������� ��������� ``` ���������</code> ������</p>")]
        public void Parse_NotSingleBackticks_Ignore(string input, string expected)
        {
            ParseCheck(input,expected);
        }
        
    }
}