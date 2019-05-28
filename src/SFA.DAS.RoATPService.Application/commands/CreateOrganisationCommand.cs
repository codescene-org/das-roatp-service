﻿using System;

namespace SFA.DAS.RoATPService.Application.Commands
{
    public class CreateOrganisationCommand
    {
        public int ProviderTypeId { get; set; }
        public int OrganisationTypeId { get; set; }
        public long Ukprn { get; set; }
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public int OrganisationStatusId { get; set; }
        public DateTime StatusDate { get; set; }
        public string CharityNumber { get; set; }
        public string CompanyNumber { get; set; }
        public bool ParentCompanyGuarantee { get; set; }
        public bool FinancialTrackRecord { get; set; }
        public bool NonLevyContract { get; set; }
        public DateTime? StartDate { get; set; }
        public string Username { get; set; }

        public bool? SourceIsUKRLP { get; set; }
    }
}
