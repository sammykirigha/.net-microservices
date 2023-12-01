using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;

        public PlatformController(IPlatformRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
        public ActionResult<PlatFormReadDto> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            var PlatformModel = _mapper.Map<Platform>(platformCreateDto);
            _repository.CreatePlatform(PlatformModel);
            _repository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatFormReadDto>(PlatformModel);
            return CreatedAtRoute(nameof(GetPlatformsById), new { Id = platformReadDto.ID }, platformReadDto);
        }
    }
}