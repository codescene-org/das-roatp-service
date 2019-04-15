using System;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Commands;
using SFA.DAS.RoATPService.Application.Interfaces;

namespace SFA.DAS.RoATPService.Application.Mappers
{
    public class MapCreateOrganisationRequestToCommand: IMapCreateOrganisationRequestToCommand
    {
        private const int MainProviderTypeId = 1;
        private const int EmployerProviderTypeId = 2;
        private const int SupportingProviderTypeId = 3;

        private const int OrganisationStatusActive = 1;
        private const int OrganisationStatusOnboarding = 3;

        public CreateOrganisationCommand Map(CreateOrganisationRequest request)
        {     
            int organisationStatusId;
            DateTime? startDate = null;

            switch (request.ProviderTypeId)
            {
                case MainProviderTypeId:
                case EmployerProviderTypeId:
                    organisationStatusId = OrganisationStatusOnboarding;
                    break;
                case SupportingProviderTypeId:
                    organisationStatusId = OrganisationStatusActive;
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
