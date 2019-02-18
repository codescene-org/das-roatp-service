namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using Domain;
    using MediatR;

    public class CreateOrganisationRequest : IRequest<bool>
    {
        public Organisation Organisation { get; set; }

        public string Username { get; set; }
    }
}
