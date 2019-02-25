namespace SFA.DAS.RoATPService.Importer
{
    using SFA.DAS.RoATPService.Importer.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRegisterImporter
    {
        Task<bool> ImportRegisterEntries(string connectionString, List<RegisterEntry> registerEntries);
    }
}
