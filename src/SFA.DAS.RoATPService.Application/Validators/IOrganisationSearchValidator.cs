
namespace SFA.DAS.RoATPService.Application.Validators
{
    public interface IOrganisationSearchValidator
    {
        bool IsValidSearchTerm(string searchTerm);
        bool IsValidUKPRN(string candidateUkPrn);
    }
}
