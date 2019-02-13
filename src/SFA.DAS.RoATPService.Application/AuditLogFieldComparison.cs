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

        public async Task<IEnumerable<AuditLogEntry>> BuildListOfFieldsChanged(Organisation originalOrganisation, Organisation updatedOrganisation)
        {
            CompareLogic organisationComparison = new CompareLogic(new ComparisonConfig
                {
                    CompareChildren = true,
                    MaxDifferences = byte.MaxValue
                }
            );
            ComparisonResult comparisonResult = organisationComparison.Compare(originalOrganisation, updatedOrganisation);
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
                    OrganisationId = updatedOrganisation.Id,
                    FieldChanged = propertyName,
                    PreviousValue = difference.Object1Value,
                    NewValue = difference.Object2Value,
                    UpdatedAt = updatedOrganisation.UpdatedAt.Value,
                    UpdatedBy = updatedOrganisation.UpdatedBy
                };
                auditLogEntries.Add(entry);
            }

            return await Task.FromResult(auditLogEntries);
        }
    }
}
