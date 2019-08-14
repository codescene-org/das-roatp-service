﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.Interfaces
{
    public interface IEventsRepository
    {
        Task<bool> AddOrganisationStatusEvents(long ukprn, int organisationStatusId, DateTime createdOn);  
    }
}
