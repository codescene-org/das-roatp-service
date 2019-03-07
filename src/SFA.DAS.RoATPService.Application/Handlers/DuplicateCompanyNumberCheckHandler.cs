namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class DuplicateCompanyNumberCheckHandler : IRequestHandler<DuplicateCompanyNumberCheckRequest, bool>
    {
        private ILogger<DuplicateCompanyNumberCheckHandler> _logger;

        private IDuplicateCheckRepository _repository;

        public DuplicateCompanyNumberCheckHandler(ILogger<DuplicateCompanyNumberCheckHandler> logger,
            IDuplicateCheckRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<bool> Handle(DuplicateCompanyNumberCheckRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.DuplicateCompanyNumberExists(request.OrganisationId, request.CompanyNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to perform company number duplicate check", ex);
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
