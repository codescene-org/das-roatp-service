namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Domain;
    using Exceptions;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.RoATPService.Application.Interfaces;
    using Validators;

    public class GetOrganisationRegisterStatusHandler : IRequestHandler<GetOrganisationRegisterStatusRequest, OrganisationRegisterStatus>
    { 
        private IOrganisationRepository _repository;
        private ILogger<GetOrganisationRegisterStatusHandler> _logger;
        private IOrganisationValidator _validator;

        public GetOrganisationRegisterStatusHandler(IOrganisationRepository repository,
            ILogger<GetOrganisationRegisterStatusHandler> logger, IOrganisationValidator validator)
        {
            _repository = repository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<OrganisationRegisterStatus> Handle(GetOrganisationRegisterStatusRequest request, CancellationToken cancellationToken)
        {
            if (!IsValidUkprn(request.UKPRN))
            {
                throw new BadRequestException("Invalid UKPRN");
            }

            return await _repository.GetOrganisationRegisterStatus(request.UKPRN);
        }

        private bool IsValidUkprn(string ukprn)
        {
            long ukprnValue = 0;
            var isNumeric = Int64.TryParse(ukprn, out ukprnValue);
            if (!isNumeric)
            {
                return false;
            }

            return _validator.IsValidUKPRN(ukprnValue);

        }
    }
}
