
using System.Collections.Generic;

namespace SFA.DAS.RoATPService.Api.Client.Models.Ukrlp
{
    public class UkprnLookupResponse
    {
        public bool Success { get; set; }
        public List<ProviderDetails> Results { get; set; }
    }
}
