using System.Collections.Generic;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Data.Helpers
{
    public interface ICacheHelper
    {
        IEnumerable<ProviderType> GetProviderTypes();
        void CacheProviderTypes(IEnumerable<ProviderType> providerTypes);
    }
}