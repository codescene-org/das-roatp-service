﻿namespace SFA.DAS.RoATPService.Application.Handlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Interfaces;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.RoATPService.Application.Exceptions;
    using Validators;

    public class UpdateOrganisationUkprnHandler
        : UpdateOrganisationHandlerBase, IRequestHandler<UpdateOrganisationUkprnRequest, bool>
    {
        private ILogger<UpdateOrganisationUkprnHandler> _logger;
        private IOrganisationValidator _validator;
        private IUpdateOrganisationRepository _updateOrganisationRepository;
        private IAuditLogRepository _auditLogRepository;

        private const string FieldChanged = "UKPRN";

        public UpdateOrganisationUkprnHandler(ILogger<UpdateOrganisationUkprnHandler> logger,
            IOrganisationValidator validator, IUpdateOrganisationRepository updateOrganisationRepository,
            IAuditLogRepository auditLogRepository)
        {
            _logger = logger;
            _validator = validator;
            _updateOrganisationRepository = updateOrganisationRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<bool> Handle(UpdateOrganisationUkprnRequest request, CancellationToken cancellationToken)
        {
            if (!_validator.IsValidUKPRN(request.Ukprn))
            {
                var invalidUkprnError = $@"Invalid Organisation Ukprn '{request.Ukprn}'";
                _logger.LogInformation(invalidUkprnError);
                throw new BadRequestException(invalidUkprnError);
            }

            var duplicateUkprnDetails = _validator.DuplicateUkprnInAnotherOrganisation(request.Ukprn, request.OrganisationId);
            
            if (duplicateUkprnDetails.DuplicateFound)
            {
                var invalidUkprnError = $@"Ukprn '{request.Ukprn}' already used against organisation '{duplicateUkprnDetails.DuplicateOrganisationName}'";
                _logger.LogInformation(invalidUkprnError);
                throw new BadRequestException(invalidUkprnError);
            }


            _logger.LogInformation($@"Handling Update '{FieldChanged}' for Organisation ID [{request.OrganisationId}]");

            long previousUkprn = await _updateOrganisationRepository.GetUkprn(request.OrganisationId);

            if (previousUkprn == request.Ukprn)
            {
                return await Task.FromResult(false);
            }

            bool success = await _updateOrganisationRepository.UpdateUkprn(request.OrganisationId, request.Ukprn, request.UpdatedBy);

            if (!success)
            {
                return await Task.FromResult(false);
            }

            var auditRecord = CreateAuditLogEntry(request.OrganisationId, request.UpdatedBy,
                FieldChanged, previousUkprn.ToString(), request.Ukprn.ToString());

            return await _auditLogRepository.WriteFieldChangesToAuditLog(auditRecord);
        }
    }
}