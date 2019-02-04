namespace SFA.DAS.RoATPService.Settings
{
    using Newtonsoft.Json;

    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired] public ApiAuthentication ApiAuthentication { get; set; }
     
        [JsonRequired] public string SqlConnectionString { get; set; }

        [JsonRequired] public string SessionRedisConnectionString { get; set; }

        [JsonRequired] public RegisterAuditLogSettings RegisterAuditLogSettings { get; set; }
    }
}