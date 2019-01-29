namespace SFA.DAS.RoATPService.Importer
{
    using System;

    public class RegisterEntry 
    {
        public int ApplicationRouteId { get; set; }

        public long UKPRN { get; set; }
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public int OrganisationTypeId { get; set; }
        public bool ParentCompanyGuarantee { get; set; }
        public bool HasNoFinTrackRecord { get; set; }
        public string Status { get; set; }

        public string StatusLongText
        {
            get
            {
                int statusId = Convert.ToInt32(Status);
                switch (statusId)
                {
                    case 0:
                        return "Inactive";
                    case 1:
                        return "Active";
                    default:
                        return "Unknown";
                }
            }
        }

        public DateTime? StatusStartDate { get; set; }
        public DateTime? StatusEndDate { get; set; }
        public int? EndReasonId { get; set; }
        public string EndReasonDescription { get; set; }
        public bool AcceptNewApprentices { get; set; }
        public DateTime? AcceptStartDate { get; set; }
        public DateTime? AcceptEndDate { get; set; }

    }
}
