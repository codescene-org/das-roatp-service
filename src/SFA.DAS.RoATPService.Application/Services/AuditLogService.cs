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
        private readonly ILookupDataRepository _lookupDataRepository;
        public AuditLogService(RegisterAuditLogSettings settings, IOrganisationRepository organisationRepository, ILookupDataRepository lookupDataRepository)
        {
            _settings = settings;
            _organisationRepository = organisationRepository;
            _lookupDataRepository = lookupDataRepository;
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
                    FieldChanged = AuditLogField.FinancialTrackRecord,
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
                    FieldChanged = AuditLogField.ParentCompanyGuarantee,
                    PreviousValue = previousParentCompanyGuarantee.ToString(),
                    NewValue = newParentCompanyGuarantee.ToString()
                };
                auditData.FieldChanges.Add(entry);
            }
            return auditData;
        }

        public AuditData AuditLegalName(Guid organisationId, string updatedBy, string newLegalName)
        {
            var auditData = new AuditData { FieldChanges = new List<AuditLogEntry>(), OrganisationId = organisationId, UpdatedAt = DateTime.Now, UpdatedBy = updatedBy };
            var previousLegalName = _organisationRepository.GetLegalName(organisationId).Result;
            if (newLegalName != previousLegalName)
            {
                var entry = new AuditLogEntry
                {
                    FieldChanged = AuditLogField.LegalName,
                    PreviousValue = previousLegalName,
                    NewValue = newLegalName
                };
                auditData.FieldChanges.Add(entry);
            }
            return auditData;
        }

        public AuditData AuditTradingName(Guid organisationId, string updatedBy, string newTradingName)
        {
            var auditData = new AuditData { FieldChanges = new List<AuditLogEntry>(), OrganisationId = organisationId, UpdatedAt = DateTime.Now, UpdatedBy = updatedBy };
            var previousTradingName = _organisationRepository.GetTradingName(organisationId).Result;

            if ((!(string.IsNullOrWhiteSpace(previousTradingName) && string.IsNullOrWhiteSpace(newTradingName))) && newTradingName != previousTradingName)
            {
                var entry = new AuditLogEntry
                {
                    FieldChanged = AuditLogField.TradingName,
                    PreviousValue = previousTradingName,
                    NewValue = newTradingName
                };
                auditData.FieldChanges.Add(entry);
            }
            return auditData;
        }

        public AuditData AuditUkprn(Guid organisationId, string updatedBy, long newUkprn)
        {
            var auditData = new AuditData { FieldChanges = new List<AuditLogEntry>(), OrganisationId = organisationId, UpdatedAt = DateTime.Now, UpdatedBy = updatedBy };
            var previousUkprn = _organisationRepository.GetUkprn(organisationId).Result;

            if (previousUkprn != newUkprn)
            {
                var entry = new AuditLogEntry
                {
                    FieldChanged = AuditLogField.Ukprn,
                    PreviousValue = previousUkprn.ToString(),
                    NewValue = newUkprn.ToString()
                };
                auditData.FieldChanges.Add(entry);
            }
            return auditData;
        }

        public AuditData AuditOrganisationStatus(Guid organisationId, string updatedBy, int newOrganisationStatusId, int? newRemovedReasonId)
        {
            var auditData = new AuditData { FieldChanges = new List<AuditLogEntry>(), OrganisationId = organisationId, UpdatedAt = DateTime.Now, UpdatedBy = updatedBy };

            var existingStatusId =  _organisationRepository.GetOrganisationStatus(organisationId).Result;
            var existingRemovedReason = _organisationRepository.GetRemovedReason(organisationId).Result;
            var newRemovedReason = _lookupDataRepository.GetRemovedReasons().Result.FirstOrDefault(x=>x.Id == newRemovedReasonId);
            var organisationStatuses = _lookupDataRepository.GetOrganisationStatuses().Result.ToList();
            var newStartDate = DateTime.Today;
            var existingStartDate = _organisationRepository.GetStartDate(organisationId).Result;

            if (existingStatusId != newOrganisationStatusId)
            {
                var entry = new AuditLogEntry
                {
                    FieldChanged = AuditLogField.OrganisationStatus,
                    PreviousValue =  organisationStatuses?.FirstOrDefault(x=>x.Id == existingStatusId)?.Status,
                    NewValue = organisationStatuses?.FirstOrDefault(x => x.Id == newOrganisationStatusId)?.Status
                };
                auditData.FieldChanges.Add(entry);
            }

            if (existingRemovedReason != newRemovedReason || newRemovedReasonId.HasValue && newRemovedReasonId.Value != existingRemovedReason.Id)
            {
                var entry = new AuditLogEntry
                {
                    FieldChanged = AuditLogField.RemovedReason,
                    PreviousValue = existingRemovedReason?.Reason ?? "Not set",
                    NewValue = newRemovedReason?.Reason ?? "Not set"
                };
                auditData.FieldChanges.Add(entry);
            }

           
            if (auditData.FieldChanges.Any() && UpdateStartDateRequired(existingStatusId, newOrganisationStatusId, newStartDate, existingStartDate))
            {
                var entry = new AuditLogEntry
                {
                    FieldChanged = AuditLogField.StartDate,
                    PreviousValue = existingStartDate?.ToShortDateString() ?? "Not set",
                    NewValue = newStartDate.ToShortDateString()
                };
                auditData.FieldChanges.Add(entry);
            }
            return auditData;
        }

        private bool UpdateStartDateRequired(int oldStatusId, int newStatusId, DateTime newStartDate, DateTime? existingStartDate)
        {
            if ((oldStatusId == OrganisationStatus.Removed || oldStatusId == OrganisationStatus.Onboarding) &&
                (newStatusId == OrganisationStatus.Active || newStatusId == OrganisationStatus.ActiveNotTakingOnApprentices)
                && (!existingStartDate.HasValue || existingStartDate.Value.Date != newStartDate.Date))
            {
                return true;
            }


            return false;
        }
    }
}
