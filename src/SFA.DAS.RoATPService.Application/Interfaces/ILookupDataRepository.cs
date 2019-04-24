using System;

namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using Domain;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILookupDataRepository
    {
        // MFCMFC  
        // GetAll, GetOne (derived from GetAll)
        // ProviderType  DONE
        // OrganisationType DONE
        // OrganisationStatus
        // RemovedReason
        // ProviderTypeOrganisationStatus
        // ProviderTypeOrganisationTypes



        Task<IEnumerable<ProviderType>> GetProviderTypes();
        Task<ProviderType> GetProviderType(int providerTypeId);

        Task<IEnumerable<OrganisationType>> GetOrganisationTypes();
        Task<OrganisationType> GetOrganisationType(int organisationTypeId);

        Task<OrganisationStatus> GetOrganisationStatus(int statusId);


        Task<IEnumerable<RemovedReason>> GetRemovedReasons();

      

        // convert all logic below to rely on the GetAll calls....
        Task<IEnumerable<OrganisationType>> GetOrganisationTypesForProviderTypeId(int? providerTypeId);
        Task<IEnumerable<OrganisationStatus>> GetOrganisationStatusesForProviderTypeId(int? providerTypeId);
        Task<bool> IsOrganisationTypeValidForOrganisation(int organisationTypeId, Guid organisationId);
        Task<bool> IsOrganisationStatusValidForOrganisation(int organisationStatusId, Guid organisationId);
    }
}
