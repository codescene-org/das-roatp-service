namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using Moq;
    using NUnit.Framework;
    using Settings;

    [TestFixture]
    public class AuditLogFieldComparisonTests
    {
        private Organisation _firstOrganisation;
        private Organisation _secondOrganisation;
        private RegisterAuditLogSettings _settings;

        [SetUp]
        public void Before_each_test()
        {
            _firstOrganisation = new Organisation
            {
                Id = Guid.NewGuid(),
                ApplicationRoute = new ApplicationRoute {Id = 1, Route = "Main "},
                LegalName = "Legal Name",
                OrganisationType = new OrganisationType {Id = 0, Type = "Unassigned"},
                TradingName = "Trading Name",
                UKPRN = 10002233,
                OrganisationData = new OrganisationData {CompanyNumber = "1111222"}
            };

            _secondOrganisation = new Organisation
            {
                Id = _firstOrganisation.Id,
                ApplicationRoute = _firstOrganisation.ApplicationRoute,
                LegalName = _firstOrganisation.LegalName,
                OrganisationType = _firstOrganisation.OrganisationType,
                TradingName = _firstOrganisation.TradingName,
                UKPRN = _firstOrganisation.UKPRN,
                OrganisationData = new OrganisationData
                {
                    CompanyNumber = _firstOrganisation.OrganisationData.CompanyNumber
                },
                UpdatedAt = DateTime.Now,
                UpdatedBy = "Test"
            };

            _settings = new RegisterAuditLogSettings
            {
                IgnoredFields = new List<string>
                {
                    "CreatedBy", "CreatedAt", "UpdatedBy", "UpdatedAt"
                },
                DisplayNames = new List<AuditLogDisplayName>
                {
                    new AuditLogDisplayName
                        {FieldName = "LegalName", DisplayName = "Legal Name"},
                    new AuditLogDisplayName
                        {FieldName = "OrganisationData.CompanyNumber", DisplayName = "Company Number"}
                }
            };
        }

        [Test]
        public void Comparison_returns_empty_list_for_identical_organisations()
        {
            var comparison = new AuditLogFieldComparison(_settings);

            var results = comparison.BuildListOfFieldsChanged(_firstOrganisation, _secondOrganisation).Result.ToList();

            results.Should().BeEmpty();
        }

        [Test]
        public void Comparison_returns_empty_list_if_only_altered_fields_are_in_ignored_list()
        {
            var comparison = new AuditLogFieldComparison(_settings);

            _secondOrganisation.CreatedAt = DateTime.Now.AddDays(-10);
            _secondOrganisation.CreatedBy = "Unit test";
            _secondOrganisation.UpdatedAt = DateTime.Now;
            _secondOrganisation.UpdatedBy = "Unit test";

            var results = comparison.BuildListOfFieldsChanged(_firstOrganisation, _secondOrganisation).Result.ToList();

            results.Should().BeEmpty();
        }

        [Test]
        public void Comparison_returns_single_field_change()
        {
            var comparison = new AuditLogFieldComparison(_settings);

            _secondOrganisation.UKPRN = 11112222;

            var results = comparison.BuildListOfFieldsChanged(_firstOrganisation, _secondOrganisation).Result.ToList();

            results.Should().HaveCount(1);

            results[0].FieldChanged.Should().Be("UKPRN");
            results[0].PreviousValue.Should().Be(_firstOrganisation.UKPRN.ToString());
            results[0].NewValue.Should().Be(_secondOrganisation.UKPRN.ToString());
            results[0].UpdatedBy.Should().Be("Test");
            results[0].OrganisationId.Should().Be(_firstOrganisation.Id);
        }

        [Test]
        public void Comparison_returns_single_field_change_with_display_name_replacement()
        {
            var comparison = new AuditLogFieldComparison(_settings);

            _secondOrganisation.LegalName = "New Legal Name";

            var results = comparison.BuildListOfFieldsChanged(_firstOrganisation, _secondOrganisation).Result.ToList();

            results.Should().HaveCount(1);

            results[0].FieldChanged.Should().Be("Legal Name");
            results[0].PreviousValue.Should().Be("Legal Name");
            results[0].NewValue.Should().Be("New Legal Name");
            results[0].UpdatedBy.Should().Be("Test");
            results[0].OrganisationId.Should().Be(_firstOrganisation.Id);
        }

        [Test]
        public void Comparison_returns_multiple_field_changes()
        {
            var comparison = new AuditLogFieldComparison(_settings);

            _secondOrganisation.UKPRN = 11112222;
            _secondOrganisation.OrganisationData.CompanyNumber = "AB112233";

            var results = comparison.BuildListOfFieldsChanged(_firstOrganisation, _secondOrganisation).Result.ToList();

            results.Should().HaveCount(2);

            results[0].FieldChanged.Should().Be("UKPRN");
            results[0].PreviousValue.Should().Be(_firstOrganisation.UKPRN.ToString());
            results[0].NewValue.Should().Be(_secondOrganisation.UKPRN.ToString());
            results[0].UpdatedBy.Should().Be("Test");
            results[0].OrganisationId.Should().Be(_firstOrganisation.Id);
            results[1].FieldChanged.Should().Be("Company Number");
            results[1].PreviousValue.Should().Be(_firstOrganisation.OrganisationData.CompanyNumber);
            results[1].NewValue.Should().Be(_secondOrganisation.OrganisationData.CompanyNumber);
            results[1].UpdatedBy.Should().Be("Test");
            results[1].OrganisationId.Should().Be(_firstOrganisation.Id);
        }

        [Test]
        public void Comparison_handles_multiple_field_changes_where_one_field_is_ignored()
        {
            var comparison = new AuditLogFieldComparison(_settings);

            _secondOrganisation.UpdatedAt = DateTime.Now;
            _secondOrganisation.OrganisationData.CompanyNumber = "AB112233";

            var results = comparison.BuildListOfFieldsChanged(_firstOrganisation, _secondOrganisation).Result.ToList();

            results.Should().HaveCount(1);

            results[0].FieldChanged.Should().Be("Company Number");
            results[0].PreviousValue.Should().Be(_firstOrganisation.OrganisationData.CompanyNumber);
            results[0].NewValue.Should().Be(_secondOrganisation.OrganisationData.CompanyNumber);
            results[0].UpdatedBy.Should().Be("Test");
            results[0].OrganisationId.Should().Be(_firstOrganisation.Id);
        }

        [Test]
        public void Comparison_handles_field_changes_where_update_fields_not_populated()
        {
            var comparison = new AuditLogFieldComparison(_settings);

            _secondOrganisation.UpdatedAt = DateTime.Now;
            _secondOrganisation.OrganisationData.CompanyNumber = "AB112233";
            _secondOrganisation.UpdatedAt = null;
            _secondOrganisation.UpdatedBy = null;

            var results = comparison.BuildListOfFieldsChanged(_firstOrganisation, _secondOrganisation).Result.ToList();

            results.Should().HaveCount(1);

            results[0].FieldChanged.Should().Be("Company Number");
            results[0].PreviousValue.Should().Be(_firstOrganisation.OrganisationData.CompanyNumber);
            results[0].NewValue.Should().Be(_secondOrganisation.OrganisationData.CompanyNumber);
            results[0].UpdatedBy.Should().Be("System");
            results[0].UpdatedAt.Should().BeCloseTo(DateTime.Now);
            results[0].OrganisationId.Should().Be(_firstOrganisation.Id);
        }
    }
}
