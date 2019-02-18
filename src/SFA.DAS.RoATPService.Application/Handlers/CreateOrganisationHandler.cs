namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Domain;
    using Exceptions;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Validators;

    public class CreateOrganisationHandler : IRequestHandler<CreateOrganisationRequest, bool>
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly ILogger<CreateOrganisationHandler> _logger;
        private readonly IOrganisationValidator _organisationValidator;

        public CreateOrganisationHandler(IOrganisationRepository repository, ILogger<CreateOrganisationHandler> logger, 
                                         IOrganisationValidator organisationValidator)
        {
            _organisationRepository = repository;
            _logger = logger;
            _organisationValidator = organisationValidator;
        }

        public Task<bool> Handle(CreateOrganisationRequest request, CancellationToken cancellationToken)
        {
            if (!IsValidCreateOrganisation(request.Organisation))
            {
                string invalidOrganisationError = $@"Invalid Organisation data";
                _logger.LogInformation(invalidOrganisationError);
                throw new BadRequestException(invalidOrganisationError);
            }

            _logger.LogInformation($@"Handling Create Organisation Search for UKPRN [{request.Organisation.UKPRN}]");

            return _organisationRepository.CreateOrganisation(request.Organisation, request.Username);
        }

        private bool IsValidCreateOrganisation(Organisation requestOrganisation)
        {
            return (_organisationValidator.IsValidLegalName(requestOrganisation.LegalName)
                    && _organisationValidator.IsValidApplicationRouteId(requestOrganisation.ApplicationRoute.Id)
                    && _organisationValidator.IsValidStatus(requestOrganisation.Status)
                    && _organisationValidator.IsValidStatusDate(requestOrganisation.StatusDate)
                    && _organisationValidator.IsValidUKPRN(requestOrganisation.UKPRN));
        }
    }
}
