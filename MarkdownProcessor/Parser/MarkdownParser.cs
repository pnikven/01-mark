using System.Linq;

namespace MarkdownProcessor.Parser
{
    class MarkdownParser
    {
        public static string Parse(string filecontent)
        {
            var paragraphs = ParagraphExtractor.ExtractParagraphs(filecontent);

            var processedParagraphs = paragraphs
                .Select(ParagraphPreprocessor.EscapeAngleBrackets)
                .Select(ParagraphPreprocessor.PreprocessParagraph)
                .Select(TagWrapper.Create(TagName.Code).Wrap)
                .Select(ParagraphPreprocessor.ReplaceUnderscoresInCodeToEntities)
                .Select(TagWrapper.Create(TagName.Em).Wrap)
                .Select(TagWrapper.Create(TagName.Strong).Wrap)
                .Select(TagWrapper.Create(TagName.P).Wrap)
                .Select(ParagraphPreprocessor.PostprocessParagraph);

            return string.Join("\n", processedParagraphs);
        }
    }
}
