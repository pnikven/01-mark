using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MarkdownProcessor
{
    class ParagraphPreprocessor
    {
        private const string UnderscoreReplacement = "<US>";
        private const string BacktickReplacement = "<BT>";

        private static readonly Dictionary<string, string> ReplacementForMark = 
            new Dictionary<string, string>()
            {
                    {"_", UnderscoreReplacement},
                    {"`", BacktickReplacement}
            };

        public static string PreprocessParagraph(string paragraph)
        {
            var preprocessedParagraph = ReplaceEscapedMarksToEntities(paragraph);
            preprocessedParagraph = ReplaceUnderscoresInTextAndDigitsToEntities(preprocessedParagraph);
            preprocessedParagraph = ReplaceMultiBackticksToEntities(preprocessedParagraph);
            preprocessedParagraph = ReplaceMultiUnderscoresToEntities(preprocessedParagraph);
            return preprocessedParagraph;
        }

        public static string ReplaceEscapedMarksToEntities(string paragraph)
        {
            return ReplacementForMark
                    .Keys
                    .Aggregate(paragraph, (currentParagraph, escapedMark) =>
                        currentParagraph.Replace(@"\" + escapedMark, @"\" + ReplacementForMark[escapedMark]));
        }

        public static string ReplaceUnderscoresInTextAndDigitsToEntities(string paragraph)
        {
            return ReplaceInvalidMarksToEntities(paragraph,
                string.Format(@"[{0}]+_+[{0}]+(_+[{0}]+)*", @"\p{L}\p{Nd}"),
                "_");
        }

        public static string ReplaceMultiBackticksToEntities(string paragraph)
        {
            return ReplaceInvalidMarksToEntities(paragraph, "``+", "`");
        }

        public static string ReplaceMultiUnderscoresToEntities(string paragraph)
        {
            return ReplaceInvalidMarksToEntities(paragraph, "___+", "_");
        }

        public static string ReplaceUnderscoresInCodeToEntities(string paragraph)
        {
            return ReplaceInvalidMarksToEntities(paragraph, "<code>.*?_+.*?</code>", "_");
        }

        private static string ReplaceInvalidMarksToEntities(string paragraph, string pattern, string mark)
        {
            var matches =
                from object match in Regex.Matches(paragraph,pattern) select match.ToString();
            return matches
                .OrderByDescending(match=>match.Length)
                .Aggregate(paragraph, (currentParagraph, match) =>
                    currentParagraph.Replace(match, match.Replace(mark, ReplacementForMark[mark])));
        }

        public static string PostprocessParagraph(string paragraph)
        {
            var result = ReplaceEntitiesToTextRepresentation(paragraph);
            return result;
        }

        public static string ReplaceEntitiesToTextRepresentation(string paragraph)
        {
            return ReplacementForMark
                    .Values
                    .Aggregate(paragraph, (currentParagraph, entity) =>
                        currentParagraph.Replace(entity, 
                            ReplacementForMark.First(v=>v.Value==entity).Key));            
        }

    }

    [TestFixture]
    class ParagraphPreprocessorTests
    {
        [Test]
        public void ReplaceUnderscoresInTextAndDigitsToEntities_UnderscoresInTextAndDigits_ReplacesToEntities()
        {
            var input = @"Подчерки_внутри_текста__и__цифр_12_3 не _считаются_ выделением _и должны_ оставаться символами_подчерка.";

            var result = ParagraphPreprocessor.ReplaceUnderscoresInTextAndDigitsToEntities(input);

            Assert.AreEqual(@"Подчерки<US>внутри<US>текста<US><US>и<US><US>цифр<US>12<US>3 не _считаются_ выделением _и должны_ оставаться символами<US>подчерка.", result);
        }

        [Test]
        public void ReplaceEscapedMarksToEntities_EscapedMarks_ReplacesToEntities()
        {
            var input = @"Some \``text\\ \wit\_h escaped\__ marks.";

            var result = ParagraphPreprocessor.ReplaceEscapedMarksToEntities(input);

            Assert.AreEqual(@"Some \<BT>`text\\ \wit\<US>h escaped\<US>_ marks.", result);
        }

        [Test]
        public void ReplaceEntitiesToTextRepresentation_Entities_ReplacesToTextRepresentation()
        {
            var input = @"Some \<BT>`text\\ \wit\<US>h entities\<US>_ <US>in text.";

            var result = ParagraphPreprocessor.ReplaceEntitiesToTextRepresentation(input);

            Assert.AreEqual(@"Some \``text\\ \wit\_h entities\__ _in text.", result);
        }

        [Test]
        public void ReplaceMultiBackticksToEntities_MultiBackticks_ReplacesToEntities()
        {
            var input = "Повторяющиеся цепочки обратных `` кавычек ``` заменяются на примитивы.";

            var result = ParagraphPreprocessor.ReplaceMultiBackticksToEntities(input);

            Assert.AreEqual("Повторяющиеся цепочки обратных <BT><BT> кавычек <BT><BT><BT> заменяются на примитивы.", result);
        }

        [Test]
        public void ReplaceMultiUnderscoresToEntities_MultiUnderscores_ReplacesToEntities()
        {
            var input = "Повторяющиеся __цепочки___ более двух __ подчерков ___заменяются ____на примитивы.";

            var result = ParagraphPreprocessor.ReplaceMultiUnderscoresToEntities(input);

            Assert.AreEqual("Повторяющиеся __цепочки<US><US><US> более двух __ подчерков <US><US><US>заменяются <US><US><US><US>на примитивы.", result);
        }

        [Test]
        public void ReplaceUnderscoresInCodeToEntities_UnderscoresInCode_ReplacesToEntities()
        {
            var input = "Предложение <code>с _подчеркиваниями_ внутри</code> _фрагментов_ , <code>заключенных __в__ тег</code> code.";

            var result = ParagraphPreprocessor.ReplaceUnderscoresInCodeToEntities(input);

            Assert.AreEqual("Предложение <code>с <US>подчеркиваниями<US> внутри</code> _фрагментов_ , <code>заключенных <US><US>в<US><US> тег</code> code.", result);
        }
    }
}
