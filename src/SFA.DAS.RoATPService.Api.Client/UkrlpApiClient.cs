namespace SFA.DAS.RoATPService.Api.Client
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using AutoMapper;
    using global::AutoMapper;
    using Interfaces;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.RoATPService.Api.Client.Models.Ukrlp;
    using SFA.DAS.RoATPService.Settings;

    public class UkrlpApiClient : IUkrlpApiClient
    {
        private readonly ILogger<UkrlpApiClient> _logger;

        private IWebConfiguration _config;

        private HttpClient _httpClient;

        private IUkrlpSoapSerializer _serializer;

        public UkrlpApiClient(ILogger<UkrlpApiClient> logger, IWebConfiguration config, HttpClient httpClient, IUkrlpSoapSerializer serializer)
        {
            _logger = logger;
            _config = config;
            _httpClient = httpClient;
            _serializer = serializer;
        }

        public async Task<UkprnLookupResponse> GetTrainingProviderByUkprn(long ukprn)
        {
            // Due to a bug in .net core, we have to parse the SOAP XML from UKRLP by hand
            // If this ever gets fixed then look to remove this code and replace with 'Add connected service'
            // https://github.com/dotnet/wcf/issues/3228

            var request = _serializer.BuildUkrlpSoapRequest(ukprn, _config.UkrlpApiAuthentication.StakeholderId,
                _config.UkrlpApiAuthentication.QueryId);

            var requestMessage =
                new HttpRequestMessage(HttpMethod.Post, _config.UkrlpApiAuthentication.ApiBaseAddress)
                {
                    Content = new StringContent(request, Encoding.UTF8, "text/xml")
                };

            var responseMessage = await _httpClient.SendAsync(requestMessage);

            if (!responseMessage.IsSuccessStatusCode)
            {
                var failureResponse = new UkprnLookupResponse
                {
                    Success = false,
                    Results = new List<ProviderDetails>()
                };
                return await Task.FromResult(failureResponse);
            }

            string soapXml = await responseMessage.Content.ReadAsStringAsync();
            var matchingProviderRecords = _serializer.DeserialiseMatchingProviderRecordsResponse(soapXml);

            ProviderDetails providerDetails = null;

            if (matchingProviderRecords != null)
            {
                providerDetails = Mapper.Map<ProviderDetails>(matchingProviderRecords);

                var result = new List<ProviderDetails>
                {
                    providerDetails
                };
                var resultsFound = new UkprnLookupResponse
                {
                    Success = true,
                    Results = result
                };
                return await Task.FromResult(resultsFound);
            }
            else
            {
                var noResultsFound = new UkprnLookupResponse
                {
                    Success = true,
                    Results = new List<ProviderDetails>()
                };
                return await Task.FromResult(noResultsFound);
            }

        }
    }
}
