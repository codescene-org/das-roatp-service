namespace SFA.DAS.RoATPService.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain;
    using Interfaces;
    using KellermanSoftware.CompareNetObjects;
    using Settings;

    public class AuditLogFieldComparison : IAuditLogFieldComparison
    {
        private RegisterAuditLogSettings _settings;

        public AuditLogFieldComparison(RegisterAuditLogSettings settings)
        {
            _settings = settings;
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
            var updatedAt = updatedOrganisation.UpdatedAt;
            if (!updatedAt.HasValue)
            {
                updatedAt = DateTime.Now;
            }

            var updatedBy = updatedOrganisation.UpdatedBy;
            if (String.IsNullOrWhiteSpace(updatedBy))
            {
                updatedBy = "System";
            }

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

                AuditLogDisplayName displayNameForProperty = _settings.DisplayNames.FirstOrDefault(x => x.FieldName == propertyName);

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
    }
}
