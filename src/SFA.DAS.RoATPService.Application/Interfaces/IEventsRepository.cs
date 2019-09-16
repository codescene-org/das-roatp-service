using System;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.Interfaces
{
    public interface IEventsRepository
    {
        Task<bool> AddOrganisationStatusEvents(long ukprn, int organisationStatusId, DateTime createdOn);

        Task<bool> AddOrganisationStatusEventsFromOrganisationId(Guid organisationId, int organisationStatusId,
            DateTime createdOn);
    }
}