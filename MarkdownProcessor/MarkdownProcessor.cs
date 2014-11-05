using System;
using System.IO;
using System.Linq;
using System.Reflection;
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

            WriteHtmlFileInProgramDirectory(filename, result);
        }

        private static string GetFilename(string[] args)
        {
            if (args.Length != 0) return args[0];
            Console.WriteLine("File name must be provided");
            return "";
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
            return string.Join(PSeparator,
                ExtractParagraphs(filecontent)
                    .Select(p => 
                        WrapP(
                        UnescapeMarks(
                        WrapStrong(
                        WrapEm(
                        WrapCodeAndEscapeMarksInCode(
                        EscapeAngleBrackets(p))))))));
        }

        private static void WriteHtmlFileInProgramDirectory(string filename, string htmlContent)
        {
            using (var sw =
                new StreamWriter(FormResultingHtmlFilename(filename), false, Encoding.UTF8))
                sw.Write(htmlContent);
        }

        private static string FormResultingHtmlFilename(string filename)
        {
            var programDirecory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var htmlFilename = Path.GetFileNameWithoutExtension(filename) + ".html";
            return Path.Combine(programDirecory, htmlFilename);
        }

        public static string[] ExtractParagraphs(string input)
        {
            if (input == null)
                throw new ArgumentNullException("input string must be provided");
            var result = Regex.Split(input, @"\n\s*\n", RegexOptions.Singleline)
                .Where(st => st != "")
                .ToArray();
            return result.Length > 0 ? result : new[] { "<p></p>" };
        }

        public static string EscapeAngleBrackets(string textForEscape)
        {
            return textForEscape.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        public static string WrapCodeAndEscapeMarksInCode(string input)
        {
            var codeWrapped = WrapCode(input);
            return EscapeMarksInCode(codeWrapped);
        }

        public static string WrapCode(string input)
        {
            return Regex.Replace(input,
                @"(?<![\\`])`(" + @"[^`]+" + @")(?<!\\)`(?!`)",
                "<code>$1</code>");
        }

        public static string EscapeMarksInCode(string textToEscape)
        {
            var escapeMarks = false;
            var result = Regex.Split(textToEscape, "(<code>|</code>)")
                .Select(st =>
                {
                    var resultString = escapeMarks ? st.Replace("_", "\\_") : st;
                    escapeMarks = st == "<code>";
                    return resultString;
                }).ToArray();
            return string.Join("", result);
        }

        public static string WrapEm(string input)
        {
            return Regex.Replace(input,
                @"(?<![\\_\w])_(" + @"(_{2,}|[^_])+" + @")(?<!\\)_(?![_\w])",
                "<em>$1</em>");
        }

        public static string WrapStrong(string input)
        {
            return Regex.Replace(input,
                @"(?<![\\_\w])__(?!_)(((?!__).)*)(?<![\\_])__(?![_\w])",
                "<strong>$1</strong>");
        }

        public static string UnescapeMarks(string textWithEscapedMarks)
        {
            return textWithEscapedMarks.Replace(@"\_", "_").Replace(@"\`", "`");
        }

        private static string WrapP(string input)
        {
            return "<p>" + input + "</p>";
        }

    }

    [TestFixture]
    public class MarkdownProcessor_should
    {
        [Test]
        public void throw_argument_null_exception_on_null()
        {
            var result = Assert.Throws<ArgumentNullException>(
                () => MarkdownProcessor.ExtractParagraphs(null));

            StringAssert.Contains("input string must be provided", result.Message);
        }

        [Test]
        public void wrap_empty_sentence_to_empty_paragraph()
        {
            var input = "";

            var result = MarkdownProcessor.ExtractParagraphs(input);

            Assert.AreEqual(new[] {"<p></p>"}, result);
        }

        [Test]
        public void wrap_one_sentence_to_paragraph()
        {
            var input = "One sentence.";

            var result = MarkdownProcessor.ExtractParagraphs(input);

            Assert.AreEqual(new [] {"One sentence."}, result);
        }

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

            var result = MarkdownProcessor.ExtractParagraphs(input);

            Assert.AreEqual(
                new[] {"One sentence.","Two sentence"},
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
        public void wrap_text_on_two_lines_between_two_underscores_to_em()
        {
            var input = "Текст _окруженный \nсимволами_ подчеркивания в нескольких строках";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual("Текст <em>окруженный \nсимволами</em> подчеркивания в нескольких строках", result);
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
        public void not_wrap_double_underscore_to_em()
        {
            var input = "Текст с двойным __ подчеркиванием";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_single_underscores_in_text_and_digits_to_em()
        {
            var input = "Подчерки_внутри_текста__и__цифр_12_3 не считаются выделением";

            var result = MarkdownProcessor.WrapEm(input);

            Assert.AreEqual(input,result);
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
        public void wrap_text_between_backticks_to_code()
        {
            var input = @"Текст окруженный `одинарными _обратными_ кавычками` -> code";

            var result = MarkdownProcessor.WrapCode(input);

            Assert.AreEqual(@"Текст окруженный <code>одинарными _обратными_ кавычками</code> -> code", result);
        }

        [Test]
        public void not_wrap_text_between_two_escaped_backticks_to_code()
        {
            var input = @"Экранирование: \`Вот это\`, не должно выделиться тегом code";

            var result = MarkdownProcessor.WrapCode(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_double_backticks_to_code()
        {
            var input = @"Текст окруженный ``двойными _обратными_ кавычками`` ->X code";

            var result = MarkdownProcessor.WrapCode(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_double_backticks_to_code()
        {
            var input = @"Текст с `` двойными _обратными_ кавычками ->X code";

            var result = MarkdownProcessor.WrapCode(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_double_backtick_and_single_backtick_to_code()
        {
            var input = @"Текст с ``двойной и одинарной обратными кавычками` ->X code";

            var result = MarkdownProcessor.WrapCodeAndEscapeMarksInCode(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void not_wrap_text_between_single_backtick_and_double_backtick_to_code()
        {
            var input = @"Текст с `одинарной и двойной обратными кавычками`` ->X code";

            var result = MarkdownProcessor.WrapCodeAndEscapeMarksInCode(input);

            Assert.AreEqual(input, result);
        }

        [Test]
        public void escape_marks_in_code_before_proceeding_processing()
        {
            var input ="Метки _внутри_ <code>тегов _кода_ code</code> __должны__ экранироваться";

            var result = MarkdownProcessor.EscapeMarksInCode(input);

            Assert.AreEqual(@"Метки _внутри_ <code>тегов \_кода\_ code</code> __должны__ экранироваться", result);
        }

        [Test]
        public void unescape_marks_finally()
        {
            var input = @"В конце \`обработки` \__необходимо_ убрать _\_экранирование_\_ всех меток";

            var result = MarkdownProcessor.UnescapeMarks(input);

            Assert.AreEqual(@"В конце `обработки` __необходимо_ убрать __экранирование__ всех меток", result);
        }

    }

}
