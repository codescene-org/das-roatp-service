using System.Data;
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Data.DapperDataHandlers
{
    public class RemovedReasonDataHandler : SqlMapper.TypeHandler<RemovedReason>
    {
        private const string RoatpDateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public override RemovedReason Parse(object value)
        {
            return JsonConvert.DeserializeObject<RemovedReason>(value.ToString(),
                new IsoDateTimeConverter() { DateTimeFormat = RoatpDateTimeFormat });
        }

        public override void SetValue(IDbDataParameter parameter, RemovedReason value)
        {
            parameter.Value = JsonConvert.SerializeObject(value,
                new IsoDateTimeConverter() { DateTimeFormat = RoatpDateTimeFormat });
        }
    }
}
