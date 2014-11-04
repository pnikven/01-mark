﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace MarkdownProcessor
{
    public class MarkdownProcessor
    {
        // used to join paragraphs in resulting html for source readability
        public const string PSeparator = "\n\n"; 

        static void Main(string[] args)
        {
            var filename = GetFilename(args);
            if (filename == "") return;

            var filecontent = ReadSourceFile(filename);

            var result = ProcessAndGetHtml(filecontent);

            WriteHtmlFile(filename, result);
        }

        private static string GetFilename(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("File name must be provided");
                return "";
            }
            return args[0];
        }

        private static string ReadSourceFile(string filename)
        {
            string filecontent;
            using (var sr = new StreamReader(filename, Encoding.UTF8))
                filecontent = sr.ReadToEnd();
            return filecontent;
        }

        private static string ProcessAndGetHtml(string filecontent)
        {
            var result = string.Join(PSeparator,
                ExtractParagraphs(filecontent)
                    .Select(p => WrapStrong(WrapEm(WrapCodeAndEscapeMarks(p)))));
            return result;
        }

        private static void WriteHtmlFile(string filename, string result)
        {
            using (var sw = new StreamWriter(filename + ".html", false, Encoding.UTF8))
                sw.Write(result);
        }

        public static string WrapDoubleNewlinesToParagraphs(string input)
        {
            if(input == null) throw new ArgumentException("input string must be provided");

            var paragraphs = ExtractParagraphs(input);

            if (paragraphs.Length == 0) return "<p></p>";

            return string.Join(PSeparator, paragraphs );
        }

        private static string[] ExtractParagraphs(string input)
        {
            return Regex.Split(input, @"\n\s*\n", RegexOptions.Singleline)
                .Where(st => st != "")
                .Select(EscapeAngleBrackets)
                .Select(p => "<p>"+p+"</p>")
                .ToArray();
        }

        private static string EscapeAngleBrackets(string st)
        {
            return st.Replace("<","&lt;").Replace(">","&gt;");
        }

        public static string WrapEm(string input)
        {
            var result = Regex.Replace(input,
                @"(?<![\\_])_(" + @"(_{2,}|[^_])+" + @")(?<!\\)_(?!_)", "<em>$1</em>");

            return result;
        }

        public static string WrapStrong(string input)
        {
            var result = Regex.Replace(input,
                @"(?<![\\_])__(?!_)(((?!__).)*)(?<![\\_])__(?!_)", "<strong>$1</strong>");

            return result;
        }

        public static string WrapCodeAndEscapeMarks(string input)
        {
            var codeWrapped = Regex.Replace(input,
                @"(?<![\\`])`(" + @"[^`]+" + @")(?<!\\)`(?!`)",
                "<code>$1</code>");
            var escapeMarks = false;
            var marksEscaped = Regex.Split(codeWrapped, "(<code>|</code>)")
                .Select(st =>
                {
                    var resultString = escapeMarks ? st.Replace("_", "\\_") : st;
                    escapeMarks = st == "<code>";
                    return resultString;
                }).ToArray();

            return string.Join("",marksEscaped);
        }

    }

    [TestFixture]
    public class MarkdownProcessor_should
    {
        [Test]
        public void throw_argument_exception_on_null()
        {
            string input = null;

            var result = Assert.Throws<ArgumentException>(
                () => MarkdownProcessor.WrapDoubleNewlinesToParagraphs(input));

            StringAssert.Contains("input string must be provided", result.Message);
        }

        [Test]
        public void wrap_empty_sentence_to_empty_paragraph()
        {
            var input = "";

            var result = MarkdownProcessor.WrapDoubleNewlinesToParagraphs(input);

            Assert.AreEqual("<p></p>", result);
        }

        [Test]
        public void wrap_one_sentence_to_paragraph()
        {
            var input = "One sentence.";

            var result = MarkdownProcessor.WrapDoubleNewlinesToParagraphs(input);

            Assert.AreEqual("<p>One sentence.</p>", result);
        }

        [Test]
        public void escape_html_tags()
        {
            var input = "<p>One sentence.</p>";

            var result = MarkdownProcessor.WrapDoubleNewlinesToParagraphs(input);

            Assert.AreEqual("<p>&lt;p&gt;One sentence.&lt;/p&gt;</p>", result);
        }

        [Test]
        public void wrap_two_sentences_to_two_paragraphs()
        {
            var input = "One sentence.\n    \nTwo sentence";

            var result = MarkdownProcessor.WrapDoubleNewlinesToParagraphs(input);

            Assert.AreEqual("<p>One sentence.</p>" + MarkdownProcessor.PSeparator + "<p>Two sentence</p>", result);
        }

        [Test]
        public void join_two_paragraphs_by_PSeparator()
        {
            var input = "One sentence.\n    \nTwo sentence";

            var result = MarkdownProcessor.WrapDoubleNewlinesToParagraphs(input);

            Assert.AreEqual("<p>One sentence.</p>" + MarkdownProcessor.PSeparator + "<p>Two sentence</p>", 
                result);
        }

        [Test]
        public void wrap_text_between_two_underscores_to_em()
        {
            var input = "Текст _окруженный с двух сторон_  одинарными символами подчерка";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual("Текст <em>окруженный с двух сторон</em>  одинарными символами подчерка", result);
        }

        [Test]
        public void not_wrap_text_between_two_double_underscores_to_em()
        {
            var input = "Текст __окруженный с двух сторон__  двойными символами подчерка";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_two_escaped_underscores_to_em()
        {
            var input = @"Экранирование: \_Вот это\_, не должно выделиться тегом <em>";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_double_underscore_and_single_underscore_to_em()
        {
            var input = @"Экранирование: __Вот это_, не должно выделиться тегом <em>";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_single_underscore_and_double_underscore_to_em()
        {
            var input = @"Экранирование: _Вот это__, не должно выделиться тегом <em>";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void wrap_text_on_two_lines_between_two_underscores_to_em()
        {
            var input = "Текст _окруженный \nсимволами_ подчеркивания в нескольких строках";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual("Текст <em>окруженный \nсимволами</em> подчеркивания в нескольких строках", result);
        }

        [Test]
        public void not_wrap_double_underscores_to_em()
        {
            var input = "Текст с двойным __ подчеркиванием";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual(input, result);
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
            var input = "Текст __с __четырьмя__ двойными__ подчеркиваниями";

            var result = MarkdownProcessor.WrapStrong(input);

            Assert.AreEqual("Текст <strong>с </strong>четырьмя<strong> двойными</strong> подчеркиваниями", result);
        }

        [Test]
        public void wrap_text_between_two_underscores_with_inner_double_underscore_to_em()
        {
            var input = "Внутри _выделения em может быть __ двойное подчеркивание_.";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual("Внутри <em>выделения em может быть __ двойное подчеркивание</em>.", result);
        }

        [Test]
        public void wrap_text_between_two_underscores_with_inner_double_underscores_to_em()
        {
            var input = "Внутри _выделения em может быть __strong__ выделение_.";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual("Внутри <em>выделения em может быть __strong__ выделение</em>.", result);
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
        public void wrap_text_between_backticks_to_code_with_escaped_marks()
        {
            var input = @"Текст окруженный `одинарными _обратными_ кавычками` -> code";

            var result = MarkdownProcessor.WrapCodeAndEscapeMarks(input);

            Assert.AreEqual(@"Текст окруженный <code>одинарными \_обратными\_ кавычками</code> -> code", result);
        }

        [Test]
        public void not_wrap_text_between_two_escaped_backticks_to_code()
        {
            var input = @"Экранирование: \`Вот это\`, не должно выделиться тегом code";

            var result = MarkdownProcessor.WrapCodeAndEscapeMarks(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_double_backticks_to_code()
        {
            var input = @"Текст окруженный ``двойными _обратными_ кавычками`` ->X code";

            var result = MarkdownProcessor.WrapCodeAndEscapeMarks(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_double_backticks_to_code()
        {
            var input = @"Текст с `` двойными _обратными_ кавычками ->X code";

            var result = MarkdownProcessor.WrapCodeAndEscapeMarks(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_double_backtick_and_single_backtick_to_code()
        {
            var input = @"Текст с ``двойной и одинарной обратными кавычками` ->X code";

            var result = MarkdownProcessor.WrapCodeAndEscapeMarks(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_single_backtick_and_double_backtick_to_code()
        {
            var input = @"Текст с `одинарной и двойной обратными кавычками`` ->X code";

            var result = MarkdownProcessor.WrapCodeAndEscapeMarks(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void escape_marks_in_code()
        {
            var input ="Метки _внутри_ `одинарных _обратных_ кавычек` __должны__ экранироваться";

            var result = MarkdownProcessor.WrapCodeAndEscapeMarks(input);

            Assert.AreEqual(@"Метки _внутри_ <code>одинарных \_обратных\_ кавычек</code> __должны__ экранироваться", result);

        }
    }


}
