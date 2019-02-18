namespace SFA.DAS.RoATPService.Domain
{
    public class OrganisationData
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Postcode { get; set; }
        public string CompanyNumber { get; set; }
        public InactiveReason InactiveReason { get; set; }
        public bool ParentCompanyGuarantee { get; set; }
        public bool HasNoFinancialTrackRecord { get; set; }
    }
}
