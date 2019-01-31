namespace SFA.DAS.RoATPService.Application.Api.StartupConfiguration
{
    using AutoMapper;
    using Domain;
    using RoATPService.Api.Types.Models;

    public static class MappingStartup
    {
        public static void AddMappings()
        {
            Mapper.Initialize(cfg => { cfg.CreateMap<Organisation, OrganisationSearchResult>(); });
        }
    }
}