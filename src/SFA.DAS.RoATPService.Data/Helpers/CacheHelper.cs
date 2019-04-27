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
            var results = _cache.Get(ProviderTypesKey);
            return (List<ProviderType>) results;
        }

        public void CacheProviderTypes(IEnumerable<ProviderType> providerTypes)
        {
            _cache.Add(ProviderTypesKey, providerTypes.ToList(), _policy);
        }

        public IEnumerable<OrganisationType> GetOrganisationTypes()
        {
            var results = _cache.Get(OrganisationTypesKey);
            return (List<OrganisationType>)results;
        }

        public void CacheOrganisationTypes(IEnumerable<OrganisationType> organisationTypes)
        {
            _cache.Add(OrganisationTypesKey, organisationTypes.ToList(), _policy);
        }

        public IEnumerable<OrganisationStatus> GetOrganisationStatuses()
        {
            var results = _cache.Get(OrganisationStatusesKey);
            return (List<OrganisationStatus>)results;
        }

        public void CacheOrganisationStatuses(IEnumerable<OrganisationStatus> organisationStatuses)
        {
            _cache.Add(OrganisationStatusesKey, organisationStatuses.ToList(), _policy);
        }

        public IEnumerable<RemovedReason> GetRemovedReasons()
        {
            var results = _cache.Get(RemovedReasonsKey);
            return (List<RemovedReason>)results;
        }

        public void CacheRemovedReasons(IEnumerable<RemovedReason> removedReasons)
        {
            _cache.Add(RemovedReasonsKey, removedReasons.ToList(), _policy);

        }

        public IEnumerable<ProviderTypeOrganisationType> GetProviderTypeOrganisationTypes()
        {
            var results = _cache.Get(ProviderTypeOrganisationTypesKey);
            return (List<ProviderTypeOrganisationType>)results;
        }

        public void CacheProviderTypeOrganisationTypes(IEnumerable<ProviderTypeOrganisationType> providerTypeOrganisationTypes)
        {
            _cache.Add(ProviderTypeOrganisationTypesKey, providerTypeOrganisationTypes.ToList(), _policy);
        }

        public IEnumerable<ProviderTypeOrganisationStatus> GetProviderTypeOrganistionStatuses()
        {
            var results = _cache.Get(ProviderTypeOrganisationStatusKey);
            return (List<ProviderTypeOrganisationStatus>)results;
        }

        public void CacheProviderTypeOrganisationStatuses(IEnumerable<ProviderTypeOrganisationStatus> providerTypeOrganisationStatuses)
        {
            _cache.Add(ProviderTypeOrganisationStatusKey, providerTypeOrganisationStatuses.ToList(), _policy);
        }

        public void PurgeAllCaches()
        {
            _cache.Remove(ProviderTypesKey);
            _cache.Remove(OrganisationTypesKey);
            _cache.Remove(OrganisationStatusesKey);
            _cache.Remove(RemovedReasonsKey);
            _cache.Remove(ProviderTypeOrganisationTypesKey);
            _cache.Remove(ProviderTypeOrganisationStatusKey);
        }
    }
}
