using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KellermanSoftware.CompareNetObjects;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Domain;
using SFA.DAS.RoATPService.Settings;

namespace SFA.DAS.RoATPService.Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly RegisterAuditLogSettings _settings;
        private readonly IOrganisationRepository _organisationRepository;
        public AuditLogService(RegisterAuditLogSettings settings, IOrganisationRepository organisationRepository)
        {
            _settings = settings;
            _organisationRepository = organisationRepository;
        }

        public async Task<AuditData> BuildListOfFieldsChanged(Organisation originalOrganisation, Organisation updatedOrganisation)
        {
            CompareLogic organisationComparison = new CompareLogic(new ComparisonConfig
                {
                    CompareChildren = true,
                    MaxDifferences = byte.MaxValue
                }
            );
            ComparisonResult comparisonResult = organisationComparison.Compare(originalOrganisation, updatedOrganisation);

            var updatedAt = updatedOrganisation.UpdatedAt ?? DateTime.Now;
            var updatedBy = string.IsNullOrWhiteSpace(updatedOrganisation.UpdatedBy) ? "System" : updatedOrganisation.UpdatedBy;

            var auditData = new AuditData
            {
                OrganisationId = updatedOrganisation.Id,
                UpdatedAt = updatedAt,
                UpdatedBy = updatedBy
            };

            List<AuditLogEntry> auditLogEntries = new List<AuditLogEntry>();
            foreach (var difference in comparisonResult.Differences)
            {
                if (_settings.IgnoredFields.Contains(difference.PropertyName))
                {
                    continue;
                }

                string propertyName = difference.PropertyName;

                AuditLogDisplayName displayNameForProperty = Enumerable.FirstOrDefault<AuditLogDisplayName>(_settings.DisplayNames, x => x.FieldName == propertyName);

                if (displayNameForProperty != null)
                {
                    propertyName = displayNameForProperty.DisplayName;
                }

                if (!updatedOrganisation.UpdatedAt.HasValue)
                {
                    updatedOrganisation.UpdatedAt = DateTime.Now;
                }

                if (String.IsNullOrWhiteSpace(updatedOrganisation.UpdatedBy))
                {
                    updatedOrganisation.UpdatedBy = "System";
                }

                AuditLogEntry entry = new AuditLogEntry
                {
                    FieldChanged = propertyName,
                    PreviousValue = difference.Object1Value,
                    NewValue = difference.Object2Value
                };
                auditLogEntries.Add(entry);

            }
            auditData.FieldChanges = auditLogEntries;

            return await Task.FromResult(auditData);
        }


        public AuditData CreateAuditLogEntry(Guid organisationId, string updatedBy, string fieldName, string oldValue,
            string newValue)
        {
            return new AuditData
            {
                OrganisationId = organisationId,
                UpdatedAt = DateTime.Now,
                UpdatedBy = updatedBy,
                FieldChanges = new List<AuditLogEntry>
                {
                    new AuditLogEntry
                    {
                        FieldChanged = fieldName,
                        NewValue = newValue,
                        PreviousValue = oldValue
                    }
                }
            };
        }

        public AuditData CreateAuditData(Guid organisationId, string updatedBy)
        {
            return new AuditData
            {
                FieldChanges = new List<AuditLogEntry>(),
                OrganisationId = organisationId,
                UpdatedAt = DateTime.Now,
                UpdatedBy = updatedBy
            };
        }

        public void AddAuditEntry(AuditData auditData, string fieldChanged, string previousValue, string newValue)
        {
            var entry = new AuditLogEntry
            {
                FieldChanged = fieldChanged,
                PreviousValue = previousValue,
                NewValue = newValue
            };

            auditData.FieldChanges.Add(entry);
        }

        public AuditData AuditFinancialTrackRecord(Guid organisationId, string updatedBy,
            bool newFinancialTrackRecord)
        {
            var auditData = new AuditData { FieldChanges = new List<AuditLogEntry>(), OrganisationId = organisationId, UpdatedAt = DateTime.Now, UpdatedBy = updatedBy };

            var previousFinancialTrackRecord = _organisationRepository.GetFinancialTrackRecord(organisationId).Result;

            if (previousFinancialTrackRecord != newFinancialTrackRecord)
            {
                var entry = new AuditLogEntry
                {
                    FieldChanged = "Financial Track Record",
                    PreviousValue = previousFinancialTrackRecord.ToString(),
                    NewValue = newFinancialTrackRecord.ToString()
                };
                auditData.FieldChanges.Add(entry);
            }
        
            return auditData;
        }

        public AuditData AuditParentCompanyGuarantee(Guid organisationId, string updatedBy, bool newParentCompanyGuarantee)
        {
            var auditData = new AuditData { FieldChanges = new List<AuditLogEntry>(), OrganisationId = organisationId, UpdatedAt = DateTime.Now, UpdatedBy = updatedBy };
            var previousParentCompanyGuarantee =  _organisationRepository.GetParentCompanyGuarantee(organisationId).Result;
            if (previousParentCompanyGuarantee != newParentCompanyGuarantee)
            {
                var entry = new AuditLogEntry
                {
                    FieldChanged = "Parent Company Guarantee",
                    PreviousValue = previousParentCompanyGuarantee.ToString(),
                    NewValue = newParentCompanyGuarantee.ToString()
                };
                auditData.FieldChanges.Add(entry);
            }
            return auditData;
        }
    }
}
