namespace SFA.DAS.RoATPService.Data.IntegrationTests
{
    using System;
    using Application.Interfaces;
    using Domain;
    using NUnit.Framework;
    using Settings;

    public class OrganisationRepositoryTests
    {
        private IWebConfiguration _configuration;
        private OrganisationRepository _repository;
        private IAuditLogRepository _auditLogRepository;

        [SetUp]
        public void Setup()
        {
            _configuration = new WebConfiguration
            {
                SqlConnectionString =
                    "Data Source=localhost\\SQLEXPRESS;Initial Catalog=SFA.DAS.RoATPService.Database;User ID=sa;Password=Password1;MultipleActiveResultSets=True;"
            };
            _auditLogRepository = new AuditLogRepository(_configuration);

            _repository = new OrganisationRepository(_configuration, _auditLogRepository);
        }
        
        [Test]
        public void Get_organisation()
        {
            Guid organisationId = new Guid("FD0E179B-CC06-48EF-9153-CDE370B83EA6");

            Organisation data = _repository.GetOrganisation(organisationId).GetAwaiter().GetResult(); 

            Assert.That(data, Is.Not.Null);
        }

        [Test]
        public void Update_organisation()
        {
            Guid organisationId = new Guid("FD0E179B-CC06-48EF-9153-CDE370B83EA6");

            Organisation data = _repository.GetOrganisation(organisationId).GetAwaiter().GetResult();

            data.ApplicationRoute.Id = 2;
            data.ApplicationRoute.Route = "New route";

            var updatedOrganisation = _repository.UpdateOrganisation(data, "Unit test").GetAwaiter().GetResult();
        }

        [Test]
        public void Create_organisation()
        {
            Organisation org = new Organisation
            {
                ApplicationRoute = new ApplicationRoute { Id = 1 },
                LegalName = "Test Ltd",
                TradingName = "Test Ltd inc",
                UKPRN = 10002222,
                OrganisationType = new OrganisationType {Id = 0},
                Status = "Live",
                StatusDate = DateTime.Now,
                OrganisationData = new OrganisationData
                {
                    CompanyNumber = "12341234",
                    ParentCompanyGuarantee = true
                }
            };

            var createOrganisation = _repository.CreateOrganisation(org, "Unit test").GetAwaiter().GetResult();
        }
    }
}