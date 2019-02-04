﻿namespace SFA.DAS.RoATPService.Settings
{
    public interface IWebConfiguration
    {
        ApiAuthentication ApiAuthentication { get; set; }
        string SqlConnectionString { get; set; }
        string SessionRedisConnectionString { get; set; }
        RegisterAuditLogSettings RegisterAuditLogSettings { get; set; }
    }
}