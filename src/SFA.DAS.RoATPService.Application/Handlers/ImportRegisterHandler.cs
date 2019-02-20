namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Exceptions;
    using Importer.Exceptions;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.RoATPService.Importer;

    public class ImportRegisterHandler : IRequestHandler<RegisterImportRequest, RegisterImportResultsResponse>
    {
        private IRegisterImportRepository _repository;
        private readonly ILogger<ImportRegisterHandler> _logger;
        
        public ImportRegisterHandler(IRegisterImportRepository repository, ILogger<ImportRegisterHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<RegisterImportResultsResponse> Handle(RegisterImportRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.ImportRegisterData(request.ContainerName, request.BlobReference);
            }
            catch (RegisterImportException importException)
            {
                throw new BadRequestException($"Invalid register import data : {importException.ImportErrorMessage}");
            }
        }
    }
}
