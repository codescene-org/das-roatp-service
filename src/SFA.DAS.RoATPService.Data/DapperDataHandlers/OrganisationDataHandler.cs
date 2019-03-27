namespace SFA.DAS.AssessorService.Data.DapperTypeHandlers
{
    using System.Data;
    using System.Runtime.Serialization;
    using Dapper;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using RoATPService.Domain;

    public class OrganisationDataHandler : SqlMapper.TypeHandler<OrganisationData>
    {
        private const string RoatpDateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public override OrganisationData Parse(object value)
        {
            return JsonConvert.DeserializeObject<OrganisationData>(value.ToString(),
                new IsoDateTimeConverter() { DateTimeFormat = RoatpDateTimeFormat });
        }

        public override void SetValue(IDbDataParameter parameter, OrganisationData value)
        {
            parameter.Value = JsonConvert.SerializeObject(value,
                new IsoDateTimeConverter() { DateTimeFormat = RoatpDateTimeFormat });
        }
    }
}
