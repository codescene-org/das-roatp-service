using System.Collections.Generic;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Data.Helpers
{
    public interface ICacheHelper
    {
        List<T> Get<T>();
        void Cache<T>(IEnumerable<T> dataList, int? minutesToCache);

        void PurgeAllCaches();
    }
}