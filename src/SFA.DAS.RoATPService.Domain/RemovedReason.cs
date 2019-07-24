namespace SFA.DAS.RoATPService.Domain
{
    public class RemovedReason : BaseEntity
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public string Description { get; set; }

        public const int Breach = 1;
        public const int ChangeOfTradingStatus = 2;
        public const int HighRiskPolicy = 3;
        public const int InadequateFinancialHealth = 4;
        public const int InadequateOfstedGrade = 5;
        public const int InternalError = 6;
        public const int Merger = 7;
        public const int MinimumStandardsNotMet = 8;
        public const int NonDirectDeliveryInTwelveMonthPeriod = 9;
        public const int ProviderError = 10;
        public const int ProviderRequest = 11;
        public const int Other = 12;
    }
}
