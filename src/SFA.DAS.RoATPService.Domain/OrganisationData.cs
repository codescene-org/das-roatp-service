﻿namespace SFA.DAS.RoATPService.Domain
{
    public class OrganisationData
    {
        public string CompanyNumber { get; set; }
        public string CharityNumber { get; set; }
        public RemovedReason RemovedReason { get; set; }
        public bool ParentCompanyGuarantee { get; set; }
        public bool FinancialTrackRecord { get; set; }
        public bool NonLevyContract { get; set; }
    }
}
