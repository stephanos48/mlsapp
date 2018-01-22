using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mlsapp.Models;
using Microsoft.Extensions.Logging;
using mlsapp.ViewModels;
using System.Collections;
using AutoMapper;
using mlsapp.Services;

namespace mlsapp.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/trips/{tripName}/stops")]
    public class StopsController : Controller
    {
        private IWorldRepository _repository;
        private ILogger<StopsController> _logger;
        private GeoCoordsService _coordsService;

        public StopsController(IWorldRepository repository, 
            ILogger<StopsController> logger,
            GeoCoordsService coordsService)
        {
            _repository = repository;
            _logger = logger;
            _coordsService = coordsService;
        }

        [HttpGet("")]
        public IActionResult Get(string tripName)
        {
            try
            {
                var trip = _repository.GetTripByName(tripName);

                return Ok(Mapper.Map<IEnumerable<StopViewModel>>(trip.Stops.OrderBy(s => s.Order).ToList()));
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get stops: {0}", ex);
            }

            return BadRequest("Failed to get stops");
        }

        [HttpPost("")]
        public async Task<IActionResult> Post(string tripName, [FromBody]StopViewModel vm)
        {
            try
            {
                var newStop = Mapper.Map<Stop>(vm);

                var result = await _coordsService.GetCoordsAsync(newStop.Name);
                if(!result.Success)
                {
                    _logger.LogError(result.Message);
                }
                else
                {
                    newStop.Latitude = result.Latitude;
                    newStop.Longitude = result.Longitude;
                }

                _repository.AddStop(tripName, newStop);

                if (await _repository.SaveChangesAsync())
                {
                    return Created($"/api/trips/{tripName}/stops/{newStop.Name}",
                        Mapper.Map<StopViewModel>(newStop));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to save new Stop: {0}", ex);
            }

            return BadRequest("Failed to save new stop");
        }
    }
}