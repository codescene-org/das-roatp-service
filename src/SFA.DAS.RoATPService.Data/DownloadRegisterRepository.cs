namespace SFA.DAS.RoATPService.Data
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using Settings;

    public class DownloadRegisterRepository : IDownloadRegisterRepository
    {
        private IWebConfiguration _configuration;
        private const string CompleteRegisterStoredProcedure = "[dbo].[RoATP_Complete_Register]";
        private const string AuditHistoryStoredProcedure = "[dbo].[RoATP_Audit_History]";
        private const string RoatpCsvSummary = "[dbo].[RoATP_CSV_SUMMARY]";

        private SqlConnection _connection;

        public DownloadRegisterRepository(IWebConfiguration configuration)
        {
            _configuration = configuration;
            _connection = new SqlConnection(_configuration.SqlConnectionString);
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetCompleteRegister()
        {
            return (await _connection.QueryAsync(CompleteRegisterStoredProcedure, commandType: CommandType.StoredProcedure)).OfType<IDictionary<string, object>>().ToList();
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetAuditHistory()
        {
            return (await _connection.QueryAsync(AuditHistoryStoredProcedure, commandType: CommandType.StoredProcedure)).OfType<IDictionary<string, object>>().ToList();
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetRoatpCsvSummary()
        {
            return (await _connection.QueryAsync(RoatpCsvSummary, commandType: CommandType.StoredProcedure)).OfType<IDictionary<string, object>>().ToList();

        }
    }
}
