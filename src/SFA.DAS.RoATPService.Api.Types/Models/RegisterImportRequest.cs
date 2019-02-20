namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using MediatR;

    public class RegisterImportRequest : IRequest<RegisterImportResultsResponse>
    {
        public string ContainerName { get; set; }
        public string BlobReference { get; set; }
    }
}
