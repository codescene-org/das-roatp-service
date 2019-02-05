namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain;

    public interface IAuditLogFieldComparison
    {
        Task<IEnumerable<AuditLogEntry>> BuildListOfFieldsChanged(Organisation originalOrganisation, Organisation updatedOrganisation);
    }
}
