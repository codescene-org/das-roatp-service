namespace SFA.DAS.RoATPService.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDownloadRegisterRepository
    {
        Task<IEnumerable<IDictionary<string, object>>> GetCompleteRegister();
        Task<IEnumerable<IDictionary<string, object>>> GetAuditHistory();
        Task<IEnumerable<IDictionary<string, object>>> GetRoatpSummary();
    }
}
