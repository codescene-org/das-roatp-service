namespace SFA.DAS.RoATPService.Importer
{
    using System;
    using System.Collections.Generic;

    public class RegisterEntryValidator
    {
        public RegisterEntryValidationResult ValidateRegisterEntry(RegisterEntry registerEntry)
        {
            var validationResult = new RegisterEntryValidationResult();

            var errorMessages = new List<string>();

            if (registerEntry.ProviderTypeId < 1 || registerEntry.ProviderTypeId > 3)
            {
                errorMessages.Add("Application route id should be between 1 and 3");
            }

            if (registerEntry.UKPRN < 10000000 || registerEntry.UKPRN > 99999999)
            {
                errorMessages.Add("UKPRN should be between 10000000 and 99999999");
            }

            if (registerEntry.OrganisationTypeId != 0)
            {
                errorMessages.Add("Unsupported organisation type id : " + registerEntry.OrganisationTypeId);
            }

            if (String.IsNullOrWhiteSpace(registerEntry.LegalName))
            {
                errorMessages.Add("Legal Name is required");
            }

            if (registerEntry.Status != "0" && registerEntry.Status != "1" && registerEntry.Status != "2")
            {
                errorMessages.Add("Unsupported status id : " + registerEntry.Status);
            }
            
            validationResult.IsValid = (errorMessages.Count == 0);
            validationResult.ValidationMessages = errorMessages;

            return validationResult;
        }
    }
}
