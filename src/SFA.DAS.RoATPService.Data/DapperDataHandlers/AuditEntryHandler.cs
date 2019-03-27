using System.Data;
using Dapper;
using Newtonsoft.Json;
using SFA.DAS.RoATPService.Domain;

namespace SFA.DAS.RoATPService.Data.DapperDataHandlers
{
    public class AuditDataHandler : SqlMapper.TypeHandler<AuditData>
    {
        public override AuditData Parse(object value)
        {
            return JsonConvert.DeserializeObject<AuditData>(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, AuditData value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }
    }
}
