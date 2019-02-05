namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using FluentAssertions;
    using NUnit.Framework;
    using Validators;

    [TestFixture]
    public class OrganisationSearchValidatorTests
    {
        private OrganisationSearchValidator _validator;

        [SetUp]
        public void Before_each_test()
        {
            _validator = new OrganisationSearchValidator();
        }

        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase(null, false)]
        [TestCase("  ", false)]
        [TestCase("A", false)]
        [TestCase("11", true)]
        [TestCase("1 ", false)]
        [TestCase("AA", true)]
        [TestCase("ABC", true)]
        public void Validator_filters_out_invalid_search_terms(string searchTerm, bool acceptedValue)
        {
            bool result = _validator.IsValidSearchTerm(searchTerm);

            result.Should().Be(acceptedValue);
        }

        [TestCase("", false)]
        [TestCase("0", false)]
        [TestCase("9999999", false)]
        [TestCase("100000000", false)]
        [TestCase("10000000", true)]
        [TestCase("99999999", true)]
        public void Validator_rejects_invalid_UKPRN_values(string ukPrn, bool acceptedValue)
        {
            bool result = _validator.IsValidUKPRN(ukPrn);

            result.Should().Be(acceptedValue);
        }
    }
}
