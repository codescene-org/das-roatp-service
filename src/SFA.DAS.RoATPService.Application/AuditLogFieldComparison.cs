namespace SFA.DAS.RoATPService.Application
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain;
    using Interfaces;
    using KellermanSoftware.CompareNetObjects;
    using Settings;

    public class AuditLogFieldComparison : IAuditLogFieldComparison
    {
        private IWebConfiguration _configuration;

        public AuditLogFieldComparison(IWebConfiguration configuration)
        {
            _configuration = configuration;
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
                if (_configuration.RegisterAuditLogSettings.IgnoredFields.Contains(difference.PropertyName))
                {
                    continue;
                }

                string propertyName = difference.PropertyName;

                AuditLogDisplayName displayNameForProperty =
                    _configuration.RegisterAuditLogSettings.DisplayNames.FirstOrDefault(x => x.FieldName == propertyName);

                if (displayNameForProperty != null)
                {
                    propertyName = displayNameForProperty.DisplayName;
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
