using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Data.Helpers
{
    public interface ICacheHelper
    {
        List<T> Get<T>();
        void Cache<T>(IEnumerable<T> dataList, int minutesToCache);
        void Cache<T>(IEnumerable<T> dataList);

        void PurgeAllCaches();
    }
}