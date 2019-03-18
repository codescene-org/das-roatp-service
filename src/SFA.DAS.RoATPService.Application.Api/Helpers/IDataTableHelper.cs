namespace SFA.DAS.RoATPService.Application.Api.Helpers
{
    using System.Collections.Generic;
    using System.Data;

    public interface IDataTableHelper
    {
        DataTable ToDataTable(IEnumerable<IDictionary<string, object>> list);
    }
}
