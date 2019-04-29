using System;
using System.Collections;
using System.Linq;
using System.Runtime.Caching;
using SFA.DAS.RoATPService.Data.Helpers;

namespace SFA.DAS.RoATPService.Data
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using Application.Interfaces;
    using Dapper;
    using Microsoft.Extensions.Logging;
    using Settings;
    using SFA.DAS.RoATPService.Domain;

    public class LookupDataRepository : ILookupDataRepository
    {
        private readonly IWebConfiguration _configuration;

        //MFCMFC add logging? Or remove logger
        private ILogger<LookupDataRepository> _logger;

        private readonly ICacheHelper _cacheHelper;
     
        public LookupDataRepository(ILogger<LookupDataRepository> logger, IWebConfiguration configuration, ICacheHelper cacheHelper)
        {
            _logger = logger;
            _configuration = configuration;
            _cacheHelper = cacheHelper;
        }

        public async Task<IEnumerable<ProviderType>> GetProviderTypes()
        {
            var results = _cacheHelper.Get<ProviderType>();
           
            if (results != null)
            {
                return await Task.FromResult(results.ToList());
            }

            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = $"select Id, ProviderType AS [Type], Description, " +
                          "CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, Status " +
                          "from [ProviderTypes] " +
                          "order by Id";

                var providerTypes = await connection.QueryAsync<ProviderType>(sql);
                _cacheHelper.Cache(providerTypes);
                

                return await Task.FromResult(providerTypes);
            }
        }

        public async Task<ProviderType> GetProviderType(int providerTypeId)
        {
            var types = await GetProviderTypes();
            return types.FirstOrDefault(x => x.Id == providerTypeId);
        }

        public async Task<IEnumerable<OrganisationType>> GetOrganisationTypes()
        {
            var results = _cacheHelper.Get<OrganisationType>();

            if (results != null)
            {
                return await Task.FromResult(results);
            }

            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = $"select ot.Id, ot.Type, ot.Description, ot.CreatedBy, ot.CreatedAt, ot.UpdatedBy, ot.UpdatedAt, ot.Status "
                          + "from [OrganisationTypes] ot " +
                          "order by Id";

                var organisationTypes = await connection.QueryAsync<OrganisationType>(sql);
                _cacheHelper.Cache(organisationTypes);
                return await Task.FromResult(organisationTypes);
            }
        }

        public async Task<OrganisationType> GetOrganisationType(int organisationTypeId)
        {
            var types = await GetOrganisationTypes();
            return types.FirstOrDefault(x => x.Id == organisationTypeId);
        }

        public async Task<IEnumerable<OrganisationStatus>> GetOrganisationStatuses()
        {
            var results = _cacheHelper.Get<OrganisationStatus>();

            if (results != null)
            {
                return await Task.FromResult(results);
            }

            var connectionString = _configuration.SqlConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = "select * FROM [OrganisationStatus] " +
                          "order by Id";

                var organisationStatuses = await connection.QueryAsync<OrganisationStatus>(sql);
                _cacheHelper.Cache(organisationStatuses);
                return await Task.FromResult(organisationStatuses);
            }
        }


        public async Task<OrganisationStatus> GetOrganisationStatus(int statusId)
        {
            var statuses = await GetOrganisationStatuses();
            return statuses.FirstOrDefault(x => x.Id == statusId);
        }


        public async Task<IEnumerable<RemovedReason>> GetRemovedReasons()
        {
            var results = _cacheHelper.Get<RemovedReason>();
            if (results != null)
            {
                return await Task.FromResult(results);
            }

            if (results != null)
            {
                return await Task.FromResult(results);
            }
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = "select Id, RemovedReason as [Reason], Status, Description, CreatedAt, " +
                          "CreatedBy, UpdatedAt, UpdatedBy FROM [RemovedReasons] " +
                          "ORDER BY Id";

                var removedReasons = await connection.QueryAsync<RemovedReason>(sql);
                _cacheHelper.Cache(removedReasons);
                return await Task.FromResult(removedReasons);
            }
        }

        public async Task<RemovedReason> GetRemovedReason(int removedReasonId)
        {
            var removedReasons = await GetRemovedReasons();
            return removedReasons.FirstOrDefault(x => x.Id == removedReasonId);
        }

        public async Task<IEnumerable<ProviderTypeOrganisationType>> GetProviderTypeOrganisationTypes()
        {
            var results = _cacheHelper.Get<ProviderTypeOrganisationType>();
            if (results != null)
            {
                return await Task.FromResult(results);
            }
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = "select Id, ProviderTypeId, OrganisationTypeId, CreatedAt, " +
                          "CreatedBy, UpdatedAt, UpdatedBy FROM [ProviderTypeOrganisationTypes] " +
                          "ORDER BY Id";

                var providerTypeOrganisationTypes = await connection.QueryAsync<ProviderTypeOrganisationType>(sql);
                _cacheHelper.Cache(providerTypeOrganisationTypes);

                return await Task.FromResult(providerTypeOrganisationTypes);
            }
        }

        public async Task<IEnumerable<ProviderTypeOrganisationStatus>> GetProviderTypeOrganisationStatuses()
        {
            var results = _cacheHelper.Get<ProviderTypeOrganisationStatus>();
            if (results != null)
            {
                return await Task.FromResult(results);
            }
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                var sql = "select Id, ProviderTypeId, OrganisationStatusId, CreatedAt, " +
                          "CreatedBy, UpdatedAt, UpdatedBy FROM [ProviderTypeOrganisationStatus] " +
                          "ORDER BY Id";

                var providerTypeOrganisationStatuses = await connection.QueryAsync<ProviderTypeOrganisationStatus>(sql);
                _cacheHelper.Cache(providerTypeOrganisationStatuses);

                return await Task.FromResult(providerTypeOrganisationStatuses);
            }
        }

        public async Task<IEnumerable<OrganisationType>> GetOrganisationTypesForProviderTypeId(int? providerTypeId)
        {
            var organisationTypes = await GetOrganisationTypes();
            if (providerTypeId == null)
                return organisationTypes;

            var providerTypeOrganisationTypes =  await GetProviderTypeOrganisationTypes();

            var selectedproviderTypeOrganisationTypes =
                providerTypeOrganisationTypes.Where(x => x.ProviderTypeId == providerTypeId);

            return organisationTypes.Where(
                x => selectedproviderTypeOrganisationTypes.Any(z => z.OrganisationTypeId == x.Id));
        }


        public async Task<IEnumerable<OrganisationStatus>> GetOrganisationStatusesForProviderTypeId(int? providerTypeId)
        {

            var organisationStatuses = await GetOrganisationStatuses();
            if (providerTypeId == null)
                return organisationStatuses;

            var providerTypeOrganisationStatuses = await GetProviderTypeOrganisationStatuses();
            var selectedProviderTypeOrganisationStatuses =
                providerTypeOrganisationStatuses.Where(x => x.ProviderTypeId == providerTypeId);

            return organisationStatuses.Where(
                x => selectedProviderTypeOrganisationStatuses.Any(z => z.OrganisationStatusId == x.Id));
        }
    }
}
