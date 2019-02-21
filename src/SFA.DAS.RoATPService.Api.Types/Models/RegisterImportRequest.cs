namespace SFA.DAS.RoATPService.Api.Types.Models
{
    using MediatR;

    public class RegisterImportRequest : IRequest<RegisterImportResultsResponse>
    {
        public string AccountName { get; set; }
        public string EndpointSuffix { get; set; }
        public string SASToken { get; set; }
        public string ContainerName { get; set; }
        public string BlobReference { get; set; }
    }
}
