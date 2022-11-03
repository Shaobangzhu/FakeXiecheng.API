using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using System.Text.RegularExpressions;
using FakeXiecheng.API.ResourceParameters;
using FakeXiecheng.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using FakeXiecheng.API.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace FakeXiecheng.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristRoutesController : ControllerBase
    {
        #region Local Variables

        private ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;

        #endregion

        #region Constructor
        public TouristRoutesController(
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccessor,
            IPropertyMappingService propertyMappingService
        )
        {
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext);
            _propertyMappingService = propertyMappingService;
        }
        #endregion

        #region Private Methods

        private string GenerateTouristRouteResourceURL(
            TouristRouteResourceParameters parameters,
            PaginationResourceParameters parameters_pagination,
            ResourceUrlType type
        )
        {
            return type switch
            {
                ResourceUrlType.PreviousPage => _urlHelper.Link("GetTouristRoutes",
                    new
                    {
                        orderBy = parameters.OrderBy,
                        keyword = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = parameters_pagination.PageNumber - 1,
                        pageSize = parameters_pagination.PageSize
                    }),
                ResourceUrlType.NextPage => _urlHelper.Link("GetTouristRoutes",
                    new
                    {
                        orderBy = parameters.OrderBy,
                        keyword = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = parameters_pagination.PageNumber + 1,
                        pageSize = parameters_pagination.PageSize
                    }),
                _ => _urlHelper.Link("GetTouristRoutes",
                    new
                    {
                        orderBy = parameters.OrderBy,
                        keyword = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = parameters_pagination.PageNumber,
                        pageSize = parameters_pagination.PageSize
                    })
            };
        }

        #endregion

        #region GET
        // api/touristRoutes?keyword=input_parameter
        [HttpGet(Name = "GetTouristRoutes")]
        [HttpHead]
        public async Task<IActionResult> GetTouristRoutes(
            [FromQuery] TouristRouteResourceParameters parameters,
            [FromQuery] PaginationResourceParameters parameters_pagination
        //[FromQuery] string keyword,
        //string rating //lessThan, largerThan, equalTo
        ) // FromQuery负责接收url的参数, FromBody负责接收请求主体
        {
            if(!_propertyMappingService.IsMappingExists<TouristRouteDto, TouristRoute>(parameters.OrderBy))
            {
                return BadRequest("Please correct input order parameter!");
            }

            var touristRoutesFromRepo = await _touristRouteRepository
                .GetTouristRoutesAsync(
                    parameters.Keyword, 
                    parameters.RatingOperator, 
                    parameters.RatingValue,
                    parameters_pagination.PageSize,
                    parameters_pagination.PageNumber,
                    parameters.OrderBy
                );
            if (touristRoutesFromRepo == null || touristRoutesFromRepo.Count() <= 0)
            {
                return NotFound("There are no tourist routes.");
            }

            var touristRoutesDto = _mapper.Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);

            var previousPageLink = touristRoutesFromRepo.HasPrevious 
                ? GenerateTouristRouteResourceURL(
                    parameters, parameters_pagination, ResourceUrlType.PreviousPage) 
                : null;

            var nextPageLink = touristRoutesFromRepo.HasNext 
                ? GenerateTouristRouteResourceURL(
                    parameters, parameters_pagination, ResourceUrlType.NextPage) 
                : null;

            #region x-pagination
            var paginationMetadata = new
            {
                previousPageLink,
                nextPageLink,
                totalCount = touristRoutesFromRepo.TotalCount,
                pageSize = touristRoutesFromRepo.PageSize,
                currentPage = touristRoutesFromRepo.CurrentPage,
                totalPages = touristRoutesFromRepo.TotalPages
            };

            Response.Headers.Add("x-pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
            #endregion

            return Ok(touristRoutesDto);
        }

        // api/touristroutes/{touristRouteId}
        [HttpGet("{touristRouteId}", Name = "GetTouristRouteById")]
        public async Task<IActionResult> GetTouristRouteById(Guid touristRouteId)
        {
            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            if (touristRouteFromRepo == null)
            {
                return NotFound($"Cannot find tourist route {touristRouteId}.");
            }

            #region DEBUG: Mock Dto
            //var touristRouteDto = new TouristRouteDto()
            //{
            //    Id = touristRouteFromRepo.Id,
            //    Title = touristRouteFromRepo.Title,
            //    Description = touristRouteFromRepo.Description,
            //    Price = touristRouteFromRepo.OriginalPrice * (decimal)(touristRouteFromRepo.DiscountPresent ?? 1),
            //    CreateTime = touristRouteFromRepo.CreateTime,
            //    UpdateTime = touristRouteFromRepo.UpdateTime,
            //    Features = touristRouteFromRepo.Features,
            //    Fees = touristRouteFromRepo.Fees,
            //    Notes = touristRouteFromRepo.Notes,
            //    Rating = touristRouteFromRepo.Rating,
            //    TravelDays = touristRouteFromRepo.TravelDays.ToString(),
            //    TripType = touristRouteFromRepo.TripType.ToString(),
            //    DepartureCity = touristRouteFromRepo.DepartureCity.ToString()
            //};
            #endregion

            var touristRouteDto = _mapper.Map<TouristRouteDto>(touristRouteFromRepo);

            return Ok(touristRouteDto);
        }
        #endregion

        #region POST
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            await _touristRouteRepository.SaveAsync();
            var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);
            return CreatedAtRoute(
                "GetTouristRouteById",
                new { touristRouteId = touristRouteToReturn.Id },
                touristRouteToReturn
            );
        }
        #endregion

        #region PUT&PATCH
        [HttpPut("{touristRouteId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTouristRoute(
            [FromRoute] Guid touristRouteId,
            [FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto
        )
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("Cannot find this tourist route!");
            }

            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            // 1.映射dto
            // 2. 更新dto
            // 3. 映射model
            _mapper.Map(touristRouteForUpdateDto, touristRouteFromRepo);

            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{touristRouteId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PartiallyUpdateTouristRoute(
            [FromRoute] Guid touristRouteId,
            [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDocument
        )
        {
            if (!(await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId)))
            {
                return NotFound("Cannot find this tourist route!");
            }

            var touristRouteFromRepo = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            var touristRouteToPatch = _mapper.Map<TouristRouteForUpdateDto>(touristRouteFromRepo);
            patchDocument.ApplyTo(touristRouteToPatch, ModelState);

            #region Validate JsonPatchDocumentation
            if (!TryValidateModel(touristRouteToPatch))
            {
                return ValidationProblem(ModelState);
            }
            #endregion

            _mapper.Map(touristRouteToPatch, touristRouteFromRepo);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }
        #endregion

        #region DELETE
        [HttpDelete("{touristRouteId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTouristRoute([FromRoute] Guid touristRouteId)
        {
            if (!await _touristRouteRepository.TouristRouteExistsAsync(touristRouteId))
            {
                return NotFound("Cannot find this tourist route!");
            }

            var touristRoute = await _touristRouteRepository.GetTouristRouteAsync(touristRouteId);
            _touristRouteRepository.DeleteTouristRoute(touristRoute);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }

        [HttpDelete("({touristIDs})")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteByIDs(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))][FromRoute] IEnumerable<Guid> touristIDs)
        {
            if(touristIDs == null)
            {
                return BadRequest();
            }

            var touristRoutesFromRepo = await _touristRouteRepository.GetTouristRoutesByIDListAsync(touristIDs);
            _touristRouteRepository.DeleteTouristRoutes(touristRoutesFromRepo);
            await _touristRouteRepository.SaveAsync();

            return NoContent();
        }
        #endregion
    }
}
