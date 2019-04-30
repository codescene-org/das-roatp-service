using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Exceptions;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Application.Validators;

namespace SFA.DAS.RoATPService.Application.Handlers
{

    public class UpdateOrganisationTradingNameHandler : IRequestHandler<UpdateOrganisationTradingNameRequest, bool>
    {
        private readonly ILogger<UpdateOrganisationTradingNameHandler> _logger;
        private readonly IOrganisationValidator _validator;
        private readonly IUpdateOrganisationRepository _updateOrganisationRepository;
        private readonly ITextSanitiser _textSanitiser;
        private readonly IAuditLogService _auditLogService;

        private const string FieldChanged = "Trading Name";

        public UpdateOrganisationTradingNameHandler(ILogger<UpdateOrganisationTradingNameHandler> logger,
            IOrganisationValidator validator, IUpdateOrganisationRepository updateOrganisationRepository, 
            ITextSanitiser textSanitiser, IAuditLogService auditLogService)
        {
            _logger = logger;
            _validator = validator;
            _updateOrganisationRepository = updateOrganisationRepository;
            _textSanitiser = textSanitiser;
            _auditLogService = auditLogService;
        }

        public async Task<bool> Handle(UpdateOrganisationTradingNameRequest request, CancellationToken cancellationToken)
        {
            var tradingName = _textSanitiser.SanitiseInputText(request.TradingName);

            if (!_validator.IsValidTradingName(tradingName))
            {
                string invalidLegalNameError = $@"Invalid Organisation Trading Name '{tradingName}'";
                _logger.LogInformation(invalidLegalNameError);
                throw new BadRequestException(invalidLegalNameError);
            }

            var auditRecord = _auditLogService.AuditTradingName(request.OrganisationId, request.UpdatedBy, tradingName);


            if (!auditRecord.ChangesMade)
            {
                return await Task.FromResult(false);
            }

            _logger.LogInformation($@"Handling Update '{FieldChanged}' for Organisation ID [{request.OrganisationId}]");
  
            var success = await _updateOrganisationRepository.UpdateTradingName(request.OrganisationId, tradingName, request.UpdatedBy);

            if (!success)
            {
                return await Task.FromResult(false);
            }

            return await _updateOrganisationRepository.WriteFieldChangesToAuditLog(auditRecord);
        }
    }
}
