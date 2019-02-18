namespace SFA.DAS.RoATPService.Settings
{
    using Newtonsoft.Json;

    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired] public ApiAuthentication ApiAuthentication { get; set; }
     
        public string SqlConnectionString { get; set; }

        public string SessionRedisConnectionString { get; set; }
        
    }
}