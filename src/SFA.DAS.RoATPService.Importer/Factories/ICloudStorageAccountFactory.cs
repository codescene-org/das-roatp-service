namespace SFA.DAS.RoATPService.Importer.Factories
{
    using Microsoft.WindowsAzure.Storage;

    public interface ICloudStorageAccountFactory
    {
        CloudStorageAccount GetStorageAccount(string sasToken, string accountName, string endpointSuffix);
    }
}
