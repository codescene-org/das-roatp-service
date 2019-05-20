using Newtonsoft.Json;

namespace SFA.DAS.RoATPService.Domain
{
    public class AuditLogEntry
    {
        public string FieldChanged { get; set; }
        public string PreviousValue { get; set; }
        public string NewValue { get; set; }

        [JsonIgnore]
        public bool IsValid => !(FieldChanged == null && PreviousValue==null && NewValue==null);
    }
}
