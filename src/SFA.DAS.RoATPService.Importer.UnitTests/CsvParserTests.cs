namespace SFA.DAS.RoATPService.Importer.UnitTests
{
    using System.IO;
    using System.Text;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class CsvParserTests
    {
        private CsvParser _parser;
        private string csvFile;

        [SetUp]
        public void Before_each_test()
        {
            _parser = new CsvParser();
            csvFile = "ProviderTypeId,UKPRN,LegalName,TradingName,OrganisationTypeId,ParentCompanyGuarantee,FinancialTrackRecord,Status,StatusDate,EndReasonId\n";
        }

        [TestCase(",10002222,Legal Name,Trading Name,0,false,false,0,30/01/2018,")]
        [TestCase("1,,Legal Name,Trading Name,0,false,false,0,30/01/2018,")]
        [TestCase("1,10002222,,Trading Name,0,false,false,0,30/01/2018,")]
        [TestCase("2,10002222,Legal Name,Trading Name,,false,false,0,30/01/2018,")]
        [TestCase("2,10002222,Legal Name,Trading Name,0,false,false,,30/01/2018,")]
        public void Parser_handles_missing_fields(string csvLine)
        {
            csvFile += csvLine;

            var stream = CreateStreamForCsv(csvFile);

            var result = _parser.ParseCsvFile(new StreamReader(stream));

            result.Entries.Count.Should().Be(0);
            result.ErrorLog.Count.Should().BeGreaterThan(0);
        }

        [Test]
        public void Parser_handles_valid_records()
        {
            csvFile += "1,10002222,Legal Name,Trading Name,0,true,true,0,30/01/2018,";

            var stream = CreateStreamForCsv(csvFile);

            var result = _parser.ParseCsvFile(new StreamReader(stream));

            result.ErrorLog.Count.Should().Be(0);
            result.Entries.Count.Should().Be(1);

            result.Entries[0].UKPRN.Should().Be(10002222);
            result.Entries[0].ProviderTypeId.Should().Be(1);
            result.Entries[0].LegalName.Should().Be("Legal Name");
            result.Entries[0].TradingName.Should().Be("Trading Name");
            result.Entries[0].OrganisationTypeId.Should().Be(0);
            result.Entries[0].ParentCompanyGuarantee.Should().Be(true);
            result.Entries[0].FinancialTrackRecord.Should().Be(true);
        }

        [TestCase("30/01/2018")]
        [TestCase("30-01-2018")]
        public void Parser_handles_different_datetime_formats(string testDate)
        {
            csvFile += "1,10002222,Legal Name,Trading Name,0,true,true,0," + testDate + ",";

            var stream = CreateStreamForCsv(csvFile);

            var result = _parser.ParseCsvFile(new StreamReader(stream));

            result.ErrorLog.Count.Should().Be(0);
            result.Entries.Count.Should().Be(1);

            result.Entries[0].UKPRN.Should().Be(10002222);
            result.Entries[0].ProviderTypeId.Should().Be(1);
            result.Entries[0].LegalName.Should().Be("Legal Name");
            result.Entries[0].TradingName.Should().Be("Trading Name");
            result.Entries[0].OrganisationTypeId.Should().Be(0);
            result.Entries[0].ParentCompanyGuarantee.Should().Be(true);
            result.Entries[0].FinancialTrackRecord.Should().Be(true);
        }

        private Stream CreateStreamForCsv(string csvFile)
        {
            byte[] csvBytes = Encoding.UTF8.GetBytes(csvFile);

            MemoryStream stream = new MemoryStream(csvBytes);

            stream.Position = 0;

            return stream;
        }
    }
}
