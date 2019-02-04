namespace SFA.DAS.RoATPService.Importer
{
    using System;

    public class RegisterEntry 
    {
        public int ProviderTypeId { get; set; }
        public long UKPRN { get; set; }
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public int OrganisationTypeId { get; set; }
        public bool ParentCompanyGuarantee { get; set; }
        public bool FinancialTrackRecord { get; set; }
        public string Status { get; set; }
        public DateTime? StatusDate { get; set; }
        public int? EndReasonId { get; set; }
    }
}
