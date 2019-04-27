namespace SFA.DAS.RoATPService.Application.Services
{
    public interface ITextSanitiser
    {
        string SanitiseInputText(string inputText);
        string StripOutHtmlTags(string inputText);
        string StripExcelFormulae(string inputText);

    }
}