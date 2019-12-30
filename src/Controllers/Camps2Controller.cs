using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreCodeCamp.Controllers
{
  [Route("api/camps")]
  [ApiController]
  [ApiVersion("2.0")]
  public class Camps2Controller : ControllerBase
  {
    private readonly ICampRepository _campRepository;
    private readonly IMapper _mapper;
    private readonly LinkGenerator _linkGenerator;

    public Camps2Controller(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
    {
      _campRepository = campRepository;
      _mapper = mapper;
      _linkGenerator = linkGenerator;
    }

    [HttpGet]
    public async Task<IActionResult> Get(bool includeTalks = false)
    {
      try
      {
        var results = await _campRepository.GetAllCampsAsync(includeTalks);
        var result = new
        {
          Count = results.Count(),
          Results = _mapper.Map<CampModel[]>(results)
        };
        return Ok(result);
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

    public async Task<ActionResult<CampModel>> Post(CampModel campModel)
    {
      var existingCamp = await _campRepository.GetCampAsync(campModel.Moniker);
      if (existingCamp != null)
      {
        return BadRequest("Moniker in Use");
      }

      var locationUrl = _linkGenerator.GetPathByAction("Get",
        "Camps",
        new  { moniker = campModel.Moniker}
        );

      if (string.IsNullOrWhiteSpace(locationUrl))
      {
        return BadRequest("Could not use current moniker");
      }

      try
      {
        var camp = _mapper.Map<Camp>(campModel);

        _campRepository.Add(camp);
        if (await _campRepository.SaveChangesAsync())
        {

          return Created(locationUrl, _mapper.Map<CampModel>(camp));
        }
      }
      catch (Exception)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
      }

      return BadRequest();
    }

    [HttpPut("{moniker}")]
    public async Task<ActionResult<CampModel>> Put(string moniker, CampModel campModel)
    {
      try
      {
        var oldCamp = await _campRepository.GetCampAsync(moniker);
        if (oldCamp == null)
        {
          return NotFound($"Could not find camp with moniker of {moniker}");
        }

        _mapper.Map(campModel, oldCamp);

        if (await _campRepository.SaveChangesAsync())
        {
          return _mapper.Map<CampModel>(oldCamp);
        }
      }
      catch (Exception)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
      }

      return BadRequest();
    }

    [HttpDelete("{moniker}")]
    public async Task<IActionResult> Delete(string moniker)
    {
      try
      {
        var oldCamp = await _campRepository.GetCampAsync(moniker);
        if (oldCamp == null)
        {
          return NotFound($"Could not find camp with moniker of {moniker}");
        }

        _campRepository.Delete(oldCamp);

        if (await _campRepository.SaveChangesAsync())
        {
          return Ok();
        }
      }
      catch (Exception)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
      }

      return BadRequest();
    }
  }
}
