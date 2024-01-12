using CommandsService.Models;

namespace CommandsService.Contracts
{
    public interface ICommandRepo 
    {
        bool SaveChanges();
        IEnumerable<Platform> GetAllPlatforms();
        void CreatePlatform(Platform platform);
        bool PlatformExists(int platformId);
        bool ExternalPlatformExists(int externalePlatformID);

        IEnumerable<Command> GetCommandsForPlatform(int platfornId);
        Command GetCommand(int platformId, int commandId);
        void CreateCommand(int platformId, Command command);
    }
}