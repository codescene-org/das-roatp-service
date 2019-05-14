namespace SFA.DAS.RoATPService.Application.Interfaces
{
    public interface ITextSanitiser
    {
        string SanitiseInputText(string inputText);
    }
}