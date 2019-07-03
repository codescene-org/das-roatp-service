using SFA.DAS.RoATPService.Api.Client.Models.Ukrlp;

namespace SFA.DAS.RoATPService.Api.Client.Interfaces
{
    public interface IUkrlpSoapSerializer
    {
        string BuildUkrlpSoapRequest(long ukprn, string stakeholderId, string queryId);
        MatchingProviderRecords DeserialiseMatchingProviderRecordsResponse(string soapXml);
    }
}
