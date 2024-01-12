using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlatformService.AsyncDataService;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataService.Http;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformController( 
            ICommandDataClient commandDataClient, 
            IPlatformRepo repository, 
            IMessageBusClient messageBusClient,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatFormReadDto>> GetPlatforms()
        {
            var platforms = _repository.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatFormReadDto>>(platforms));
        }

        [HttpGet("{id}", Name = "GetPlatformsById")]
        public ActionResult<PlatFormReadDto> GetPlatformsById(int id)
        {
            var platform = _repository.GetPlatformById(id);
            if (platform != null)
            {
                return Ok(_mapper.Map<PlatFormReadDto>(platform));
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatFormReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            var PlatformModel = _mapper.Map<Platform>(platformCreateDto);
            _repository.CreatePlatform(PlatformModel);
            _repository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatFormReadDto>(PlatformModel);

            //send Sync Message
            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
                
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"---> Could not send synchronously: {ex.Message}");
            }

            //sent async message
            try
            {
                var publishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                publishedDto.Event = "Platform_Published";
                _messageBusClient.PublishNewPlatform(publishedDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"---> Could not send synchronously: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformsById), new { Id = platformReadDto.ID }, platformReadDto);
        }
    }
}