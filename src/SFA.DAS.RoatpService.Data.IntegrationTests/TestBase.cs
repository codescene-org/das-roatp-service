using NUnit.Framework;
using SFA.DAS.RoatpService.Data.IntegrationTests.Services;

namespace SFA.DAS.RoatpService.Data.IntegrationTests
{
    public class TestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            new DatabaseService().SetupDatabase();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            new DatabaseService().DropDatabase();
        }
    }
}
