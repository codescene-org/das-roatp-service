
namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using Domain;
    using FluentAssertions;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class OrganisationDataSerialisationTests
    {
        [Test]
        public void Reason_data_json_parses_to_object()
        {
            var reasonJson =
                "{\"Id\":5,\"Reason\":\"Inadequate Ofsted grade\",\"Description\":null,\"CreatedBy\":\"System\",\"CreatedAt\":\"2019-02-11 15:47:23\",\"UpdatedBy\":null,\"Status\":\"Live\"}";

            RemovedReason reasonData = JsonConvert.DeserializeObject<RemovedReason>(reasonJson);

            reasonData.Should().NotBeNull();
            reasonData.Id.Should().Be(5);
            reasonData.Reason.Should().Be("Inadequate Ofsted grade");
            reasonData.CreatedAt.Year.Should().Be(2019);
            reasonData.CreatedAt.Month.Should().Be(2);
            reasonData.CreatedAt.Day.Should().Be(11);
            reasonData.Status.Should().Be("Live");
        }

        [Test]
        public void Organisation_data_json_parses_to_object()
        { 
            var json = "{ \"CompanyNumber\":\"12345678\",\"CharityNumber\":\"1234567\",\"ParentCompanyGuarantee\":false,\"FinancialTrackRecord\":true,\"NonLevyContract\":false,\"StartDate\":\"2019-03-27 00:00:00\",\"RemovedReason\":{\"Id\":5,\"Reason\":\"Inadequate Ofsted grade\",\"Description\":null,\"CreatedBy\":\"System\",\"CreatedAt\":\"2019-02-11 15:47:23\",\"UpdatedBy\":null,\"UpdatedAt\":null,\"Status\":\"Live\"}}";

            OrganisationData data = JsonConvert.DeserializeObject<OrganisationData>(json);

            data.Should().NotBeNull();
            data.CompanyNumber.Should().Be("12345678");
            data.CharityNumber.Should().Be("1234567");
            data.ParentCompanyGuarantee.Should().BeFalse();
            data.FinancialTrackRecord.Should().BeTrue();
            data.NonLevyContract.Should().BeFalse();
            data.StartDate.Should().Be(new DateTime(2019, 03, 27));
            data.RemovedReason.Id.Should().Be(5);
        }
    }
}
