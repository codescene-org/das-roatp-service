using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Data.Helpers
{
    public class CacheHelper:ICacheHelper
    {
        private const string ProviderTypesKey = "ProviderTypes";
        private const string OrganisationTypesKey = "OrganisationTypes";
        private const string OrganisationStatusesKey = "OrganisationStatuses";
        private const string RemovedReasonsKey = "RemovedReasons";
        private const string ProviderTypeOrganisationTypesKey = "ProviderTypeOrganisationTypes";
        private const string ProviderTypeOrganisationStatusKey = "ProviderTypeOrganisationStatusKey";

        private readonly MemoryCache _cache = MemoryCache.Default;
        private readonly CacheItemPolicy _policy = new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 0, 10, 0) };

        public IEnumerable<ProviderType> GetProviderTypes()
        {
        
            var res = _cache.Get(ProviderTypesKey);
            return (List<ProviderType>) res;
        }

        public void CacheProviderTypes(IEnumerable<ProviderType> providerTypes)
        {
            _cache.Add(ProviderTypesKey, providerTypes.ToList(), _policy);
        }
    }
}
