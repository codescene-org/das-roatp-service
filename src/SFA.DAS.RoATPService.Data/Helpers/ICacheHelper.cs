using System.Collections.Generic;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Data.Helpers
{
    public interface ICacheHelper
    {
        IEnumerable<ProviderType> GetProviderTypes();
        void CacheProviderTypes(IEnumerable<ProviderType> providerTypes);

        IEnumerable<OrganisationType> GetOrganisationTypes();
        void CacheOrganisationTypes(IEnumerable<OrganisationType> organisationTypes);


        IEnumerable<OrganisationStatus> GetOrganisationStatuses();
        void CacheOrganisationStatuses(IEnumerable<OrganisationStatus> organisationStatuses);

        IEnumerable<RemovedReason> GetRemovedReasons();
        void CacheRemovedReasons(IEnumerable<RemovedReason> removedReasons);


        IEnumerable<ProviderTypeOrganisationType> GetProviderTypeOrganisationTypes();
        void CacheProviderTypeOrganisationTypes(IEnumerable<ProviderTypeOrganisationType> providerTypeOrganisationTypes);


        IEnumerable<ProviderTypeOrganisationStatus> GetProviderTypeOrganistionStatuses();
        void CacheProviderTypeOrganisationStatuses(IEnumerable<ProviderTypeOrganisationStatus> providerTypeOrganisationStatuses);
        void PurgeAllCaches();
    }
}