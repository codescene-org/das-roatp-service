using System;

namespace SFA.DAS.RoATPService.Data
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using Settings;
    using SFA.DAS.RoATPService.Application.Interfaces;

    public class DownloadRegisterRepository : IDownloadRegisterRepository
    {
        private IWebConfiguration _configuration;
        private const string CompleteRegisterStoredProcedure = "[dbo].[RoATP_Complete_Register]";
        private const string AuditHistoryStoredProcedure = "[dbo].[RoATP_Audit_History]";
        private const string RoatpCsvSummary = "[dbo].[RoATP_CSV_SUMMARY]";
        
        public DownloadRegisterRepository(IWebConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetCompleteRegister()
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                return (await connection.QueryAsync(CompleteRegisterStoredProcedure, 
                    commandType: CommandType.StoredProcedure)).OfType<IDictionary<string, object>>().ToList();
            }
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetAuditHistory()
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                return (await connection.QueryAsync(AuditHistoryStoredProcedure,
                    commandType: CommandType.StoredProcedure)).OfType<IDictionary<string, object>>().ToList();
            }
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetRoatpSummary()
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                return (await connection.QueryAsync(RoatpCsvSummary, commandType: CommandType.StoredProcedure))
                    .OfType<IDictionary<string, object>>().ToList();
            }
        }
        
        public async Task<IEnumerable<IDictionary<string, object>>> GetRoatpSummaryUkprn(int ukprn)
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                
                return (await connection.QueryAsync(RoatpCsvSummary, new {ukprn},
                    commandType: CommandType.StoredProcedure)).OfType<IDictionary<string, object>>().ToList();
            }
        }

        public async Task<DateTime?> GetLatestNonOnboardingOrganisationChangeDate()
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                const string sql = 
                    "select max(coalesce(updatedAt,createdAt)) latestDate from organisations o WHERE o.StatusId IN (0,1,2) ";
                return await connection.ExecuteScalarAsync<DateTime?>(sql);
            }
        }
    }
}
