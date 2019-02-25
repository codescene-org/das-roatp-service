namespace SFA.DAS.RoATPService.Importer.UnitTests
{
    using FluentAssertions;
    using NUnit.Framework;
    using SFA.DAS.RoATPService.Importer.Models;

    public class RegisterEntryValidatorTests
    {
        private RegisterEntry _registerEntry;
        private RegisterEntryValidator _validator;

        [SetUp]
        public void Before_each_test()
        {
            _registerEntry = new RegisterEntry
            {
                UKPRN = 10002222,
                ProviderTypeId = 1,
                OrganisationTypeId = 0,
                LegalName = "Training Company",
                Status = "1"
            };

            _validator = new RegisterEntryValidator();
        }

        [TestCase(1000)]
        [TestCase(9999999)]
        [TestCase(100000000)]
        public void Validator_rejects_invalid_UKPRN(long ukPrn)
        {
            _registerEntry.UKPRN = ukPrn;

            var result = _validator.ValidateRegisterEntry(_registerEntry);

            result.IsValid.Should().Be(false);
            result.ValidationMessages.Count.Should().Be(1);
        }

        [TestCase(0)]
        [TestCase(4)]
        public void Validator_rejects_invalid_provider_type(int providerTypeId)
        {
            _registerEntry.ProviderTypeId = providerTypeId;

            var result = _validator.ValidateRegisterEntry(_registerEntry);

            result.IsValid.Should().Be(false);
            result.ValidationMessages.Count.Should().Be(1);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void Validator_rejects_invalid_organisation_type(int organisationTypeId)
        {
            _registerEntry.OrganisationTypeId = organisationTypeId;

            var result = _validator.ValidateRegisterEntry(_registerEntry);

            result.IsValid.Should().Be(false);
            result.ValidationMessages.Count.Should().Be(1);
        }

        [TestCase("-1")]
        [TestCase("3")]
        [TestCase(null)]
        [TestCase("")]
        public void Validator_rejects_invalid_status_id(string statusId)
        {
            _registerEntry.Status = statusId;

            var result = _validator.ValidateRegisterEntry(_registerEntry);

            result.IsValid.Should().Be(false);
            result.ValidationMessages.Count.Should().Be(1);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Validator_rejects_invalid_legal_name(string legalName)
        {
            _registerEntry.LegalName = legalName;

            var result = _validator.ValidateRegisterEntry(_registerEntry);

            result.IsValid.Should().Be(false);
            result.ValidationMessages.Count.Should().Be(1);
        }

        [Test]
        public void Validator_handles_multiple_invalid_fields()
        {
            _registerEntry.LegalName = string.Empty;
            _registerEntry.UKPRN = 10;

            var result = _validator.ValidateRegisterEntry(_registerEntry);

            result.IsValid.Should().Be(false);
            result.ValidationMessages.Count.Should().Be(2);
        }
    }
}