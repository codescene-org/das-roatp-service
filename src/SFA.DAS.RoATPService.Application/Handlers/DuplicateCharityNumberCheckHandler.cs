namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class DuplicateCharityNumberCheckHandler : IRequestHandler<DuplicateCharityNumberCheckRequest, DuplicateCheckResponse>
    {
        private ILogger<DuplicateCharityNumberCheckHandler> _logger;

        private IDuplicateCheckRepository _repository;

        public DuplicateCharityNumberCheckHandler(ILogger<DuplicateCharityNumberCheckHandler> logger,
            IDuplicateCheckRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<DuplicateCheckResponse> Handle(DuplicateCharityNumberCheckRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.DuplicateCharityNumberExists(request.OrganisationId, request.CharityNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to perform charity number duplicate check", ex);
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
