using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFA.DAS.RoATPService.Application.Interfaces;

namespace SFA.DAS.RoATPService.Application.Validators
{
    public class OrganisationCategoryValidator: IOrganisationCategoryValidator
    {
        private readonly ILookupDataRepository _lookupRepository;

        public OrganisationCategoryValidator(ILookupDataRepository lookupRepository)
        {
            _lookupRepository = lookupRepository;
        }

        public bool IsValidCategoryId(int categoryId)
        {
            var validCategoryIds = _lookupRepository.GetValidOrganisationCategoryIds().Result;
            return validCategoryIds.Any(x => x == categoryId);
        }
    }
}
