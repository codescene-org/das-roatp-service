namespace SFA.DAS.RoATPService.Application.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Api.Types.Models;
    using Domain;
    using Exceptions;
    using FluentAssertions;
    using Handlers;
    using Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using Validators;

    [TestFixture]
    public class UpdateOrganisationHandlerTests
    {
        private UpdateOrganisationRequest _request;
        private UpdateOrganisationHandler _handler;
        private Mock<IOrganisationRepository> _organisationRepository;
        private Mock<IAuditLogFieldComparison> _fieldComparison;
        private Mock<IAuditLogRepository> _auditLogRepository;
        private Mock<ILookupDataRepository> _lookupDataRepository;
        private Mock<IDuplicateCheckRepository> _duplicationCheckRepository;

        [SetUp]
        public void Before_each_test()
        {
            _request = new UpdateOrganisationRequest
            {
                Username = "testuser",
                Organisation = new Organisation
                {
                    Id = Guid.NewGuid(),
                    ProviderType = new ProviderType { Id = 1, Type = "Main Provider"},
                    UKPRN = 10001234,
                    LegalName = "Trainer Legal Name",
                    OrganisationData = new OrganisationData(),
                    OrganisationType = new OrganisationType {Id = 0, Type = "Unassigned"},
                    Status = "Live",
                    StatusDate = DateTime.Now,
                    OrganisationStatus = new OrganisationStatus { Id = 1, Status = "Active" },
                    TradingName = "Trainer Trading Name"
                }
            };

            _organisationRepository = new Mock<IOrganisationRepository>();
            _duplicationCheckRepository = new Mock<IDuplicateCheckRepository>();
            Mock<ILogger<UpdateOrganisationHandler>> logger = new Mock<ILogger<UpdateOrganisationHandler>>();
            _fieldComparison = new Mock<IAuditLogFieldComparison>();
            _auditLogRepository = new Mock<IAuditLogRepository>();

            _lookupDataRepository = new Mock<ILookupDataRepository>();

            _handler = new UpdateOrganisationHandler(_organisationRepository.Object, logger.Object, new OrganisationValidator(_duplicationCheckRepository.Object),
                                                     _fieldComparison.Object, _auditLogRepository.Object);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Update_organisation_not_performed_if_legal_name_invalid(string legalName)
        {
            _request.Organisation.LegalName = legalName;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(4)]
        public void Update_organisation_not_performed_if_provider_type_invalid(int providerTypeId)
        {
            _request.Organisation.ProviderType = new ProviderType { Id = providerTypeId };

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [TestCase(999999)]
        [TestCase(1)]
        [TestCase(100000001)]
        public void Update_organisation_not_performed_if_UKPRN_invalid(long ukPRN)
        {
            _request.Organisation.UKPRN = ukPRN;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
         }

        [TestCase(3)]
        [TestCase(-1)]
        public void Update_organisation_not_performed_if_status_invalid(int status)
        {
            _request.Organisation.OrganisationStatus.Id = status;

            Func<Task> result = async () => await
                _handler.Handle(_request, new CancellationToken());
            result.Should().Throw<BadRequestException>();
        }

        [Test]
        public void Update_organisation_writes_to_organisation_and_audit_history()
        {
            Organisation originalOrganisation = _request.Organisation;

            _request.Organisation.TradingName = "ANDERSON TRAINING LTD";

            _organisationRepository.Setup(x => x.GetOrganisation(It.IsAny<Guid>())).ReturnsAsync(originalOrganisation);

            _organisationRepository.Setup(x => x.UpdateOrganisation(It.IsAny<Organisation>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var auditData = new AuditData
            {
                OrganisationId = _request.Organisation.Id,
                UpdatedAt = DateTime.Now,
                UpdatedBy = "testuser",
                FieldChanges = new List<AuditLogEntry>
                {
                    new AuditLogEntry
                    {
                        FieldChanged = "Trading Name",
                        PreviousValue = "Trainer Trading Name",
                        NewValue = "ANDERSON TRAINING LTD"
                    }
                }
            };
            
            _fieldComparison.Setup(x => x.BuildListOfFieldsChanged(It.IsAny<Organisation>(), It.IsAny<Organisation>()))
                .ReturnsAsync(auditData);

            _auditLogRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()))
                .ReturnsAsync(true);

            bool updateOrganisationResult = _handler.Handle(_request, new CancellationToken()).GetAwaiter().GetResult();

            updateOrganisationResult.Should().BeTrue();
        }

        [Test]
        public void Update_organisation_does_not_update_audit_history_if_no_records_affected()
        {
            Organisation originalOrganisation = _request.Organisation;

            _organisationRepository.Setup(x => x.GetOrganisation(It.IsAny<Guid>())).ReturnsAsync(originalOrganisation);

            _organisationRepository.Setup(x => x.UpdateOrganisation(It.IsAny<Organisation>(), It.IsAny<string>()))
                .ReturnsAsync(false).Verifiable();

            List<AuditLogEntry> auditLogEntries = new List<AuditLogEntry>();
            var auditData = new AuditData
            {
                FieldChanges = auditLogEntries
            };

            _fieldComparison.Setup(x => x.BuildListOfFieldsChanged(It.IsAny<Organisation>(), It.IsAny<Organisation>()))
            .ReturnsAsync(auditData);

            _auditLogRepository.Setup(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()))
                .ReturnsAsync(false).Verifiable();

            bool updateOrganisationResult = _handler.Handle(_request, new CancellationToken()).GetAwaiter().GetResult();

            updateOrganisationResult.Should().BeFalse();
            _organisationRepository.Verify(x => x.UpdateOrganisation(It.IsAny<Organisation>(), It.IsAny<string>()),
                Times.Never);
            _auditLogRepository.Verify(x => x.WriteFieldChangesToAuditLog(It.IsAny<AuditData>()),
                Times.Never);
        }
    }
}
