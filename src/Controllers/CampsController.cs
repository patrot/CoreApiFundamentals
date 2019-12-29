using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreCodeCamp.Controllers
{
  [Route("api/[controller]")]
  public class CampsController : ControllerBase
  {
    private readonly ICampRepository _campRepository;
    private readonly IMapper _mapper;

    public CampsController(ICampRepository campRepository, IMapper mapper)
    {
      _campRepository = campRepository;
      _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<CampModel[]>> Get(bool includeTalks = false)
    {
      try
      {
        var results = await _campRepository.GetAllCampsAsync(includeTalks);

        return _mapper.Map<CampModel[]>(results);
      }
      catch (Exception)
      {
       return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
      }
    }

    [HttpGet("{moniker}")]
    public async Task<ActionResult<CampModel>> Get(string moniker)
    {
      try
      {
        var result = await _campRepository.GetCampAsync(moniker);

        if (result == null) return NotFound();

        return _mapper.Map<CampModel>(result);
      }
      catch (Exception)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
      }
    }

    [HttpGet("search")]
    public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime date, bool includeTalks = false)
    {
      try
      {
        var results = await _campRepository.GetAllCampsByEventDate(date, includeTalks);

        if (!results.Any()) return NotFound();

        return _mapper.Map<CampModel[]>(results);
      }
      catch (Exception)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
      }
    }
  }
}
