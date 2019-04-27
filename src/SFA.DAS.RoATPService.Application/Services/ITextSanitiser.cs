namespace SFA.DAS.RoATPService.Application.Services
{
    public interface ITextSanitiser
    {
        string SanitiseInputText(string inputText);
    }
}