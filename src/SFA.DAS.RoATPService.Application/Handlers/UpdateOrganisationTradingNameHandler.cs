using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Exceptions;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Application.Validators;

namespace SFA.DAS.RoATPService.Application.Handlers
{

    public class UpdateOrganisationTradingNameHandler : UpdateOrganisationHandlerBase, IRequestHandler<UpdateOrganisationTradingNameRequest, bool>
    {
        private readonly ILogger<UpdateOrganisationTradingNameHandler> _logger;
        private readonly IOrganisationValidator _validator;
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;

        private const string FieldChanged = "Trading Name";

        public UpdateOrganisationTradingNameHandler(ILogger<UpdateOrganisationTradingNameHandler> logger,
            IOrganisationValidator validator, IUpdateOrganisationRepository updateOrganisationRepository)
        {
            _logger = logger;
            _validator = validator;
            _updateOrganisationRepository = updateOrganisationRepository;
        }

        public async Task<bool> Handle(UpdateOrganisationTradingNameRequest request, CancellationToken cancellationToken)
        {
            if (!_validator.IsValidTradingName(request.TradingName))
            {
                string invalidLegalNameError = $@"Invalid Organisation Trading Name '{request.TradingName}'";
                _logger.LogInformation(invalidLegalNameError);
                throw new BadRequestException(invalidLegalNameError);
            }

            string previousTradingName = await _updateOrganisationRepository.GetTradingName(request.OrganisationId);

            if ((String.IsNullOrWhiteSpace(previousTradingName) && String.IsNullOrWhiteSpace(request.TradingName)) ||
                (previousTradingName == request.TradingName))
            {
                return await Task.FromResult(false);
            }

            _logger.LogInformation($@"Handling Update '{FieldChanged}' for Organisation ID [{request.OrganisationId}]");

            request.TradingName = HtmlTagRemover.StripOutTags(request.TradingName);

            var success = await _updateOrganisationRepository.UpdateTradingName(request.OrganisationId, request.TradingName, request.UpdatedBy);

            if (!success)
            {
                return await Task.FromResult(false);
            }

            var auditRecord = CreateAuditLogEntry(request.OrganisationId, request.UpdatedBy,
                FieldChanged, previousTradingName, request.TradingName);

            return await _updateOrganisationRepository.WriteFieldChangesToAuditLog(auditRecord);
        }
    }
}
