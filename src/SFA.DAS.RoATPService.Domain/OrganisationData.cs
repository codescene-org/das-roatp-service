namespace SFA.DAS.RoATPService.Domain
{
    using System.ComponentModel;

    public class OrganisationData
    {
        [DisplayName("Address Line 1")]
        public string Address1 { get; set; }
        [DisplayName("Address Line 2")]
        public string Address2 { get; set; }
        [DisplayName("Address Line 3")]
        public string Address3 { get; set; }
        [DisplayName("Address Line 4")]
        public string Address4 { get; set; }
        public string Postcode { get; set; }
        [DisplayName("Companies House Number")]
        public string CompanyNumber { get; set; }
        [DisplayName("Charities Commission Number")]
        public InactiveReason InactiveReason { get; set; }
        [DisplayName("Parent Company Guarantee")]
        public bool ParentCompanyGuarantee { get; set; }
        [DisplayName("Has No Financial Track Record")]
        public bool HasNoFinancialTrackRecord { get; set; }
    }
}
