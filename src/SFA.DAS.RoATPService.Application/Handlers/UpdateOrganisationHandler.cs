namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Domain;
    using Exceptions;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Validators;

    public class UpdateOrganisationHandler : IRequestHandler<UpdateOrganisationRequest, bool>
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly ILogger<UpdateOrganisationHandler> _logger;
        private readonly IOrganisationValidator _organisationValidator;

        public UpdateOrganisationHandler(IOrganisationRepository repository, ILogger<UpdateOrganisationHandler> logger, 
                                         IOrganisationValidator organisationValidator)
        {
            _organisationRepository = repository;
            _logger = logger;
            _organisationValidator = organisationValidator;
        }

        public Task<bool> Handle(UpdateOrganisationRequest request, CancellationToken cancellationToken)
        {
            if (!IsValidUpdateOrganisation(request.Organisation))
            {
                string invalidSearchTermError = $@"Invalid Organisation data";
                _logger.LogInformation(invalidSearchTermError);
                throw new BadRequestException(invalidSearchTermError);
            }

            _logger.LogInformation($@"Handling Update Organisation Search for UKPRN [{request.Organisation.UKPRN}]");

            return _organisationRepository.UpdateOrganisation(request.Organisation, request.Username);
        }

        private bool IsValidUpdateOrganisation(Organisation requestOrganisation)
        {
            return (_organisationValidator.IsValidLegalName(requestOrganisation.LegalName)
                    && _organisationValidator.IsValidTradingName(requestOrganisation.TradingName)
                    && _organisationValidator.IsValidApplicationRouteId(requestOrganisation.ApplicationRoute.Id)
                    && _organisationValidator.IsValidStatus(requestOrganisation.Status)
                    && _organisationValidator.IsValidStatusDate(requestOrganisation.StatusDate)
                    && _organisationValidator.IsValidUKPRN(requestOrganisation.UKPRN));
        }
    }
}
