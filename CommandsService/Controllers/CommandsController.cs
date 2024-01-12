using AutoMapper;
using CommandsService.Contracts;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController: ControllerBase
    {
        private ICommandRepo _repository;
        private IMapper _mapper;

        public CommandsController(ICommandRepo repository, IMapper mapper)
      {
        _repository = repository;
        _mapper = mapper;
      }

      [HttpGet]
      public ActionResult<IEnumerable<CommandReadDto>> GetAllCommandsForPlatform(int platformId)
      {
        if(!_repository.PlatformExists(platformId))
        {
            return NotFound();
        }
        var commands = _repository.GetCommandsForPlatform(platformId);
        return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
      }

      [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
      public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
      {
        Console.WriteLine($"==> hitting the command service {platformId}, {commandId}");
        if(!_repository.PlatformExists(platformId))
        {
            return NotFound();
        }

        var command = _repository.GetCommand(platformId, commandId);
        if(command == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<CommandReadDto>(command));
      }

      [HttpPost]
      public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
      {
        Console.WriteLine($"==> hitting the command service {platformId}");

        if(!_repository.PlatformExists(platformId))
        {
            return NotFound();
        }

        var commandToCreate = _mapper.Map<Command>(commandDto);
        _repository.CreateCommand(platformId, commandToCreate);
        _repository.SaveChanges();      

        var commandReadDto = _mapper.Map<CommandReadDto>(commandToCreate);

        return CreatedAtRoute(nameof(GetCommandForPlatform), new {platformId = platformId, commandId = commandReadDto.Id, commandReadDto} );
        }
    }
}