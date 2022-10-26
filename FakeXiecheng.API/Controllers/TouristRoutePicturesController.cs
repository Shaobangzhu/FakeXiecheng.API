using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FakeXiecheng.API.Services;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Models;

namespace FakeXiecheng.API.Controllers
{
    [Route("api/touristRoutes/{touristRouteId}/pictures")]
    [ApiController]
    public class TouristRoutePicturesController : ControllerBase
    {
        #region Local Variables
        private ITouristRouteRepository _touristRouteRepository;
        private IMapper _mapper;
        #endregion

        #region Constructor
        public TouristRoutePicturesController(
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper
        )
        {
            _touristRouteRepository = touristRouteRepository ??
                throw new ArgumentNullException(nameof(touristRouteRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }
        #endregion

        #region GET
        [HttpGet]
        public async Task<IActionResult> GetPictureListForTouristRoute(Guid touristRouteId)
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("This tourist route doesn't exist.");
            }

            var picturesFromRepo = await _touristRouteRepository.GetPicturesByTouristRouteIdAsync(touristRouteId);

            if (picturesFromRepo == null || picturesFromRepo.Count() <= 0)
            {
                return NotFound("Picture doesn't exists.");
            }

            return Ok(_mapper.Map<IEnumerable<TouristRoutePictureDto>>(picturesFromRepo));
        }

        [HttpGet("{pictureId}", Name ="GetPicture")]
        public async Task<IActionResult> GetPicture(Guid touristRouteId, int pictureId)
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("This tourist route doesn't exist.");
            }

            var pictureFromRepo = await _touristRouteRepository.GetPictureAsync(pictureId);
            if (pictureFromRepo == null)
            {
                return NotFound("This picture doesn't exist.");
            }
            return Ok(_mapper.Map<TouristRoutePictureDto>(pictureFromRepo));
        }
        #endregion

        #region POST
        [HttpPost]
        public async Task<IActionResult> CreateTouristRoutePicture(
            [FromRoute] Guid touristRouteId,
            [FromBody] TouristRoutePictureForCreationDto touristRoutePictureForCreationDto
        )
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("This tourist route doesn't exist.");
            }

            var pictureModel = _mapper.Map<TouristRoutePicture>(touristRoutePictureForCreationDto);
            _touristRouteRepository.AddTouristRoutePicture(touristRouteId, pictureModel);
            await _touristRouteRepository.SaveAsync();
            var pictureToReturn = _mapper.Map<TouristRoutePictureDto>(pictureModel);
            return CreatedAtRoute(
                "GetPicture",
                new { 
                    touristRouteId = pictureModel.TouristRouteId,
                    pictureId = pictureModel.Id
                },
                pictureToReturn
            );
        }
        #endregion

        #region DELETE
        [HttpDelete("{pictureId}")]
        public async Task<IActionResult> DeletePicture(
            [FromRoute]Guid touristRouteId, 
            [FromRoute]int pictureId
        )
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("This tourist route doesn't exist.");
            }

            var picture = await _touristRouteRepository.GetPictureAsync(pictureId);
            _touristRouteRepository.DeleteTouristRoutePicture(picture);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }
        #endregion
    }
}
