using PlatformService.Dtos;

namespace PlatformService.SyncDataService.Http
{
    public interface ICommandDataClient
    {
        Task SendPlatformToCommand(PlatFormReadDto platFormReadDto);
    }
}