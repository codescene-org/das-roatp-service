namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using MediatR;
    using SFA.DAS.RoATPService.Domain;

    public class GetOrganisationRegisterStatusRequest : IRequest<OrganisationRegisterStatus>
    {
        public string UKPRN { get; set; }
    }
}
