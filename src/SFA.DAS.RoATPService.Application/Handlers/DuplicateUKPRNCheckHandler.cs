namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class DuplicateUKPRNCheckHandler : IRequestHandler<DuplicateUKPRNCheckRequest, bool>
    {
        private ILogger<DuplicateUKPRNCheckHandler> _logger;

        private IDuplicateCheckRepository _repository;

        public DuplicateUKPRNCheckHandler(ILogger<DuplicateUKPRNCheckHandler> logger,
            IDuplicateCheckRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<bool> Handle(DuplicateUKPRNCheckRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.DuplicateUKPRNExists(request.OrganisationId, request.UKPRN);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to perform UKPRN duplicate check", ex);
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
