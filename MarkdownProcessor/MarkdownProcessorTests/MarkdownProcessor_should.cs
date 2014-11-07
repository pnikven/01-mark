using System;
using NUnit.Framework;

namespace MarkdownProcessor
{
    [TestFixture]
    public class MarkdownProcessor_should
    {


        [Test]
        public void escape_html_tags()
        {
            var input = "<p>One sentence.</p>";

            var result = MarkdownProcessor.EscapeAngleBrackets(input);

            Assert.AreEqual("&lt;p&gt;One sentence.&lt;/p&gt;", result);
        }

        [Test]
        public void wrap_two_sentences_to_two_paragraphs()
        {
            var input = "One sentence.\n    \nTwo sentence";

            var result = ParagraphsExtractor.ExtractParagraphs(input);

            Assert.AreEqual(
                new[] {"One sentence.","Two sentence"},
                result);
        }

        [Test]
        public void wrap_text_between_two_underscores_to_em()
        {
            var input = "����� _���������� � ���� ������_  ���������� ��������� ��������";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual("����� <em>���������� � ���� ������</em>  ���������� ��������� ��������", result);
        }

        [Test]
        public void wrap_text_on_two_lines_between_two_underscores_to_em()
        {
            var input = "����� _���������� \n���������_ ������������� � ���������� �������";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual("����� <em>���������� \n���������</em> ������������� � ���������� �������", result);
        }

        [Test]
        public void wrap_text_between_two_underscores_with_inner_double_underscore_to_em()
        {
            var input = "������ _��������� em ����� ���� __ ������� �������������_.";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual("������ <em>��������� em ����� ���� __ ������� �������������</em>.", result);
        }

        [Test]
        public void wrap_text_between_two_underscores_with_inner_double_underscores_to_em()
        {
            var input = "������ _��������� em ����� ���� __strong__ ���������_.";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual("������ <em>��������� em ����� ���� __strong__ ���������</em>.", result);
        }

        [Test]
        public void not_wrap_text_between_two_double_underscores_to_em()
        {
            var input = "����� __���������� � ���� ������__  �������� ��������� ��������";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_two_escaped_underscores_to_em()
        {
            var input = @"�������������: \_��� ���\_, �� ������ ���������� ����� <em>";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_double_underscore_and_single_underscore_to_em()
        {
            var input = @"�������������: __��� ���_, �� ������ ���������� ����� <em>";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_single_underscore_and_double_underscore_to_em()
        {
            var input = @"�������������: _��� ���__, �� ������ ���������� ����� <em>";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_double_underscore_to_em()
        {
            var input = "����� � ������� __ ��������������";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_single_underscores_in_text_and_digits_to_em()
        {
            var input = "��������_������_������__�__����_12_3 �� ��������� ����������";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual(input,result);
        }

        [Test]
        public void wrap_text_between_two_double_underscores_to_strong()
        {
            var input = "����� � __����� ���������__ � �.�. ������";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual("����� � <strong>����� ���������</strong> � �.�. ������", result);
        }

        [Test]
        public void wrap_text_between_first_two_double_underscores_to_strong()
        {
            var input = "����� � __�����__ ��������__ ���������������";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual("����� � <strong>�����</strong> ��������__ ���������������", result);
        }

        [Test]
        public void wrap_text_between_first_and_second_two_double_underscores_to_strong()
        {
            var input = "����� __� __ �������� __ ��������__ ���������������";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual("����� <strong>� </strong> �������� <strong> ��������</strong> ���������������", result);
        }

        [Test]
        public void wrap_text_between_double_underscores_within_single_underscores_to_strong()
        {
            var input = "������ _��������� em ����� ���� __strong__ ���������_.";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual("������ _��������� em ����� ���� <strong>strong</strong> ���������_.", result);
        }

        [Test]
        public void wrap_text_between_double_underscores_with_inner_single_underscore_to_strong()
        {
            var input = "������ __��������� strong ����� ���� _ ���������__ �������������.";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual("������ <strong>��������� strong ����� ���� _ ���������</strong> �������������.", result);
        }

        [Test]
        public void wrap_text_between_double_underscores_with_inner_single_underscores_to_strong()
        {
            var input = "������ __��������� strong ����� ���� _em_ ���������__.";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual("������ <strong>��������� strong ����� ���� _em_ ���������</strong>.", result);
        }

        [Test]
        public void not_wrap_text_between_two_escaped_double_underscores_to_strong()
        {
            var input = @"�������������: \__��� ���\__, �� ������ ���������� ����� strong";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_double_underscore_and_triple_underscore_to_strong()
        {
            var input = @"������� �������������: __��� ���___, �� ������ ���������� ����� strong";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_triple_underscore_and_double_underscore_to_strong()
        {
            var input = @"������� �������������: ___��� ���__, �� ������ ���������� ����� strong";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_double_underscores_in_text_and_digits_to_strong()
        {
            var input = "��������_������_������__�__����_12_3 �� ��������� ����������";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void wrap_text_between_backticks_to_code()
        {
            var input = @"����� ���������� `���������� _���������_ ���������` -> code";

            var result = MarkdownProcessor.WrapCode(input);

            Assert.AreEqual(@"����� ���������� <code>���������� _���������_ ���������</code> -> code", result);
        }

        [Test]
        public void not_wrap_text_between_two_escaped_backticks_to_code()
        {
            var input = @"�������������: \`��� ���\`, �� ������ ���������� ����� code";

            var result = MarkdownProcessor.WrapCode(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_double_backticks_to_code()
        {
            var input = @"����� ���������� ``�������� _���������_ ���������`` ->X code";

            var result = MarkdownProcessor.WrapCode(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_double_backticks_to_code()
        {
            var input = @"����� � `` �������� _���������_ ��������� ->X code";

            var result = MarkdownProcessor.WrapCode(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_double_backtick_and_single_backtick_to_code()
        {
            var input = @"����� � ``������� � ��������� ��������� ���������` ->X code";

            var result = MarkdownProcessor.WrapCodeAndEscapeMarksInCode(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_single_backtick_and_double_backtick_to_code()
        {
            var input = @"����� � `��������� � ������� ��������� ���������`` ->X code";

            var result = MarkdownProcessor.WrapCodeAndEscapeMarksInCode(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void escape_marks_in_code_before_proceeding_processing()
        {
            var input ="����� _������_ <code>����� _����_ code</code> __������__ ��������������";

            var result = MarkdownProcessor.EscapeMarksInCode(input);

            Assert.AreEqual(@"����� _������_ <code>����� \_����\_ code</code> __������__ ��������������", result);
        }

        [Test]
        public void unescape_marks_finally()
        {
            var input = @"� ����� \`���������` \__����������_ ������ _\_�������������_\_ ���� �����";

            var result = MarkdownProcessor.UnescapeMarks(input);

            Assert.AreEqual(@"� ����� `���������` __����������_ ������ __�������������__ ���� �����", result);
        }

    }
}