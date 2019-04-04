using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Commands;
using SFA.DAS.RoATPService.Application.Interfaces;

namespace SFA.DAS.RoATPService.Application.Mappers
{
    public class MapCreateOrganisationRequestToCommand: IMapCreateOrganisationRequestToCommand
    {
        public CreateOrganisationCommand Map(CreateOrganisationRequest request)
        {
            const int mainProviderTypeId = 1;
            const int employerProviderTypeId = 2;
            const int supportingProviderTypeId = 3;

            const int organisationStatusOnboarding = 3;
            const int organisationStatusActive = 1;

            int organisationStatusId;
            DateTime? startDate = null;

            switch (request.ProviderTypeId)
            {
                case mainProviderTypeId:
                case employerProviderTypeId:
                    organisationStatusId = organisationStatusOnboarding;
                    break;
                case supportingProviderTypeId:
                    organisationStatusId = organisationStatusActive;
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
                Username = request.Username
            };

            return command;
        }
    }
}
