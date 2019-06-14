using System;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Commands;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Application.Mappers
{
    public class MapCreateOrganisationRequestToCommand: IMapCreateOrganisationRequestToCommand
    {
        public CreateOrganisationCommand Map(CreateOrganisationRequest request)
        {     
            int organisationStatusId;
            DateTime? startDate = null;

            switch (request.ProviderTypeId)
            {
                case ProviderType.MainProvider:
                case ProviderType.EmployerProvider:
                    organisationStatusId = OrganisationStatus.Onboarding;
                    break;
                case ProviderType.SupportingProvider:
                    organisationStatusId = OrganisationStatus.Active;
                    startDate = DateTime.Today;
                    break;

                default:
                    throw new Exception($"Provider Type {request.ProviderTypeId} not recognised");
            }
        
        var command = new CreateOrganisationCommand
            {
                CharityNumber = request.CharityNumber,
                CompanyNumber = request.CompanyNumber,
                FinancialTrackRecord = request.FinancialTrackRecord,
                LegalName = request.LegalName,
                NonLevyContract = request.NonLevyContract,
                OrganisationStatusId = organisationStatusId,
                OrganisationTypeId = request.OrganisationTypeId,
                ParentCompanyGuarantee = request.ParentCompanyGuarantee,
                ProviderTypeId = request.ProviderTypeId,
                StatusDate = request.StatusDate,
                Ukprn = request.Ukprn,
                TradingName = request.TradingName,
                StartDate = startDate,
                Username = request.Username,
                SourceIsUKRLP = request.SourceIsUKRLP,
                ApplicationDeterminedDate = request.ApplicationDeterminedDate
            };

            return command;
        }
    }
}
