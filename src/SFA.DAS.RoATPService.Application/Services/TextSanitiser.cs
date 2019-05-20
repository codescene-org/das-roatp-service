using SFA.DAS.RoATPService.Application.Interfaces;

namespace SFA.DAS.RoATPService.Application.Services
{
    public class TextSanitiser: ITextSanitiser
    {
        public string SanitiseInputText(string inputText)
        {
            var text = inputText;

            text = StripOutHtmlTags(text);

            text = StripExcelFormulae(text);

            return text;
        }

        private static string StripOutHtmlTags(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
                return inputText;

            var text = inputText;

            var rx = new System.Text.RegularExpressions.Regex("<[^>]*>");
            text = rx.Replace(text, string.Empty);

            while (text.Contains("<"))
            {
                text = text + ">";
                text = rx.Replace(text, string.Empty);
            }

            return text;
        }

        private static string StripExcelFormulae(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
                return inputText;

            var text = inputText;

            text = text.Replace("=", string.Empty);

            return text;
        }
    }
}
