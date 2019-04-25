using System;

namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using Domain;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILookupDataRepository
    {
        Task<IEnumerable<ProviderType>> GetProviderTypes();
        Task<ProviderType> GetProviderType(int providerTypeId);

        Task<IEnumerable<OrganisationType>> GetOrganisationTypes();
        Task<OrganisationType> GetOrganisationType(int organisationTypeId);

        Task<IEnumerable<OrganisationStatus>> GetOrganisationStatuses();

        Task<OrganisationStatus> GetOrganisationStatus(int statusId);


        Task<IEnumerable<RemovedReason>> GetRemovedReasons();
        Task<RemovedReason> GetRemovedReason(int removedReasonId);

        Task<IEnumerable<ProviderTypeOrganisationType>> GetProviderTypeOrganisationTypes();
        Task<IEnumerable<ProviderTypeOrganisationStatus>> GetProviderTypeOrganisationStatuses();



        // convert all logic below to rely on the GetAll calls....
        Task<IEnumerable<OrganisationType>> GetOrganisationTypesForProviderTypeId(int? providerTypeId);
        Task<IEnumerable<OrganisationStatus>> GetOrganisationStatusesForProviderTypeId(int? providerTypeId);
        //Task<bool> IsOrganisationTypeValidForOrganisation(int organisationTypeId, Guid organisationId);
        //Task<bool> IsOrganisationStatusValidForOrganisation(int organisationStatusId, Guid organisationId);
    }
}
