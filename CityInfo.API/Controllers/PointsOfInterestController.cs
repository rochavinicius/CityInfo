using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        private NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private IMailService _mailService;
        private ICityInfoRepository _repository;

        public PointsOfInterestController(IMailService mailService, ICityInfoRepository repository)
        {
            _mailService = mailService;
            _repository = repository;
        }

        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                if (!_repository.CityExists(cityId))
                {
                    return NotFound();
                }

                var pointsOfInterest = _repository.GetPointsOfInterestForCity(cityId);

                var pointsOfInterestResult = AutoMapper.Mapper.Map<IEnumerable<PointOfInterestDTO>>(pointsOfInterest);

                return Ok(pointsOfInterestResult);
            }
            catch(Exception ex)
            {
                _logger.Log(NLog.LogLevel.Fatal, ex, $"Exception while getting points of interest for ity with id {cityId}.");
                return StatusCode(500, "An error ocurred while handling your request.");
            }
        }

        [HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointsOfInterest(int cityId, int id)
        {
            if (!_repository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = _repository.GetPointOfInterestForCity(cityId, id);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            var pointOfInterestResult = AutoMapper.Mapper.Map<PointOfInterestDTO>(pointOfInterest);

            return Ok(pointOfInterestResult);
        }

        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterest(int cityId,
                [FromBody] PointOfInterestForCreationDTO pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            if (pointOfInterest.Name == pointOfInterest.Description)
            {
                ModelState.AddModelError("Description", "The description must not be equal to the name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_repository.CityExists(cityId))
            {
                return NotFound();
            }

            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfinterest).Max(p => p.Id);

            var finalPointOfInterest = AutoMapper.Mapper.Map<PointOfInterest>(pointOfInterest);

            _repository.AddPointOfInterestForCity(cityId, finalPointOfInterest);

            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            var createdPointOfInterest = AutoMapper.Mapper.Map<PointOfInterestDTO>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new { cityId = cityId, id = createdPointOfInterest.Id }, createdPointOfInterest);
        }

        [HttpPut("{cityId}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id,
                [FromBody] PointOfInterestForUpdateDTO pointOfInterest)
        {
            if (pointOfInterest == null)
            {
                return BadRequest();
            }

            if (pointOfInterest.Name == pointOfInterest.Description)
            {
                ModelState.AddModelError("Description", "The description must not be equal to the name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_repository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestToUpdate = _repository.GetPointOfInterestForCity(cityId, id);

            if (pointOfInterestToUpdate == null)
            {
                return NotFound();
            }

            AutoMapper.Mapper.Map(pointOfInterest, pointOfInterestToUpdate);

            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpPatch("{cityId}/pointsofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
                    [FromBody] JsonPatchDocument<PointOfInterestForUpdateDTO> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!_repository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestToUpdate = _repository.GetPointOfInterestForCity(cityId, id);

            if (pointOfInterestToUpdate == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = AutoMapper.Mapper.Map<PointOfInterestForUpdateDTO>(pointOfInterestToUpdate);

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pointOfInterestToPatch.Name == pointOfInterestToPatch.Description)
            {
                ModelState.AddModelError("Description", "The description must not be equal to the name.");
            }

            TryValidateModel(pointOfInterestToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AutoMapper.Mapper.Map(pointOfInterestToPatch, pointOfInterestToUpdate);

            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpDelete("{cityId}/pointsofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            if (!_repository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestToDelete = _repository.GetPointOfInterestForCity(cityId, id);

            if (pointOfInterestToDelete == null)
            {
                return NotFound();
            }

            _repository.DeletePointOfInterest(pointOfInterestToDelete);

            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            _mailService.Send("Point of interest deleted.",
                $"Point of interest {pointOfInterestToDelete.Name} with id {pointOfInterestToDelete.Id} was deleted.");

            return NoContent();
        }
    }
}
