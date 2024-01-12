using AutoMapper;
using CommandsService.Contracts;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private ICommandRepo _repository;
        private IMapper _mapper;

        public PlatformsController(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetAllPLatforms()
        {
            Console.WriteLine("===> Reading platforms from commandService");
            var platforms = _repository.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }
       [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("--> Inbound testing");
            return Ok("Inbound test of from Platforms Controller");
        }
    }
}