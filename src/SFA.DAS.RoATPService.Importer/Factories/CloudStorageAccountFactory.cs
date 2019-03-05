namespace SFA.DAS.RoATPService.Importer.Factories
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Auth;
    using System;

    public class CloudStorageAccountFactory : ICloudStorageAccountFactory
    {
        private IConfiguration _configuration;

        public CloudStorageAccountFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public CloudStorageAccount GetStorageAccount(string sasToken, string accountName, string endpointSuffix)
        {
            CloudStorageAccount storageAccount = null;
            if (_configuration["EnvironmentName"].ToUpper() == "LOCAL")
            {
                CloudStorageAccount.TryParse(_configuration["ConfigurationStorageConnectionString"], out storageAccount);

                return storageAccount;
            }

            if (String.IsNullOrWhiteSpace(sasToken) || String.IsNullOrWhiteSpace(accountName) || String.IsNullOrWhiteSpace(endpointSuffix))
            {
                return null;
            }

            StorageCredentials credentials = new StorageCredentials(sasToken);
            storageAccount = new CloudStorageAccount(credentials, accountName, endpointSuffix, true);

            return storageAccount;
        }
    }
}
