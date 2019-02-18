namespace SFA.DAS.RoATPService.Settings
{
    using System.Collections.Generic;

    public class RegisterAuditLogSettings
    {
        public List<string> IgnoredFields { get; set; }

        public List<AuditLogDisplayName> DisplayNames { get; set; }
    }

    public class AuditLogDisplayName
    {
        public string FieldName { get; set; }
        public string DisplayName { get; set; }
    }
}
