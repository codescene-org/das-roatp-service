namespace SFA.DAS.AssessorService.Data.DapperTypeHandlers
{
    using System.Data;
    using Dapper;
    using Newtonsoft.Json;
    using RoATPService.Domain;

    public class OrganisationDataHandler : SqlMapper.TypeHandler<OrganisationData>
    {
        public override OrganisationData Parse(object value)
        {
            return JsonConvert.DeserializeObject<OrganisationData>(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, OrganisationData value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }
    }
}
