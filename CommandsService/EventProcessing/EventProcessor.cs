using System.Text.Json;
using AutoMapper;
using CommandsService.Contracts;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private IServiceScopeFactory _serviceScopeFactory;
        private IMapper _mapper;

        public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            switch (eventType)
            {
                case EventType.PlatformPublished:
                    //save to db
                    addPlatform(message);
                    break;
                default:
                    Console.WriteLine("Platform published Event Detected");
                    break;
            }

        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("==>Determing event");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch (eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("Platform published Event Detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("Could not determine event type");
                    return EventType.Undetermined;
            }
        }

        private void addPlatform(string platformPublishedMessage)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                var platformPublishedDto = JsonSerializer.Deserialize<PlaformPublishedDto>(platformPublishedMessage);

                try
                {
                    var plat = _mapper.Map<Platform>(platformPublishedDto);
                    if(!repo.ExternalPlatformExists(plat.ExternalID))
                    {
                       repo.CreatePlatform(plat);
                       repo.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("==>Platform already exists");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not add platform to DB {ex.Message}");
                }
            }
        }

    }
    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}