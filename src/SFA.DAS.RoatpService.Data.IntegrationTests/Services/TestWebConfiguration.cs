using System;
using SFA.DAS.RoATPService.Settings;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Services
{
    public class TestWebConfiguration : IWebConfiguration
    {
        public ApiAuthentication ApiAuthentication
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }  

        public string SqlConnectionString { get; set; }
     
        public string SessionRedisConnectionString
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public UkrlpApiAuthentication UkrlpApiAuthentication
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }
}
