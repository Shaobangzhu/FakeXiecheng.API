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
using Microsoft.Net.Http.Headers;
using System.Dynamic;

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

        #region 创建URL请求资源参数
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
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        keyword = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = parameters_pagination.PageNumber - 1,
                        pageSize = parameters_pagination.PageSize
                    }),
                ResourceUrlType.NextPage => _urlHelper.Link("GetTouristRoutes",
                    new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        keyword = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = parameters_pagination.PageNumber + 1,
                        pageSize = parameters_pagination.PageSize
                    }),
                _ => _urlHelper.Link("GetTouristRoutes",
                    new
                    {
                        fields = parameters.Fields,
                        orderBy = parameters.OrderBy,
                        keyword = parameters.Keyword,
                        rating = parameters.Rating,
                        pageNumber = parameters_pagination.PageNumber,
                        pageSize = parameters_pagination.PageSize
                    })
            };
        }
        #endregion

        #region 给单一资源创建超媒体链接
        private IEnumerable<LinkDto> CreateLinkForTouristRoute(
            Guid touristRouteId,
            string fields
        )
        {
            var links = new List<LinkDto>();

            // 获取
            links.Add(
                new LinkDto(
                Url.Link("GetTouristRouteById", new { touristRouteId, fields }),
                "self",
                "GET"
                )
            );

            // 更新
            links.Add(
                new LinkDto(
                Url.Link("UpdateTouristRoute", new { touristRouteId }),
                "update",
                "PUT"
                )
            );

            // 局部更新
            links.Add(
                new LinkDto(
                Url.Link("PartiallyUpdateTouristRoute", new { touristRouteId }),
                "partially_update",
                "PATCH"
                )
            );

            // 删除
            links.Add(
                new LinkDto(
                Url.Link("DeleteTouristRoute", new { touristRouteId }),
                "delete",
                "DELETE"
                )
            );

            // 获取旅游路线图片
            links.Add(
                new LinkDto(
                Url.Link("GetPictureListForTouristRoute", new { touristRouteId }),
                "get_pictures",
                "GET"
                )
            );

            // 添加新的旅游路线图片
            links.Add(
                new LinkDto(
                Url.Link("CreateTouristRoutePicture", new { touristRouteId }),
                "create_pictures",
                "POST"
                )
            );

            return links;
        }
        #endregion

        #region 给列表资源创建超媒体链接
        private IEnumerable<LinkDto> CreateLinksForTouristRouteList(
            TouristRouteResourceParameters parameters,
            PaginationResourceParameters parameters_pagination)
        {
            var links = new List<LinkDto>();
            // 添加self, 自我链接
            links.Add(new LinkDto(
                    GenerateTouristRouteResourceURL(parameters, parameters_pagination, ResourceUrlType.CurrentPage),
                    "self",
                    "GET"
                ));

            // "api/touristRoutes"
            // 添加创建旅游路线
            links.Add(new LinkDto(
                    Url.Link("CreateTouristRoute", null),
                    "create_tourist_route",
                    "POST"
                ));

            return links;
        }
        #endregion

        #endregion

        #region GET
        // api/touristRoutes?keyword=传入的参数
        // 1. application/json -> 普通旅游路线资源
        // 2. application/vnd.{公司名称}.hateoas+json
        // 3. application/vnd.clu.touristRoute.simplify+json -> 输出简化版资源数据
        // 4. application/vnd.clu.touristRoute.simplify.hateoas+json -> 输出简化版hateoas超媒体资源数据
        [Produces(
            "application/json",
            "application/vnd.clu.hateoas+json",
            "application/vnd.clu.touristRoute.simplify+json",
            "application/vnd.clu.touristRoute.simplify.hateoas+json"
        )]
        [HttpGet(Name = "GetTouristRoutes")]
        [HttpHead]
        public async Task<IActionResult> GetTouristRoutes(
            [FromQuery] TouristRouteResourceParameters parameters,
            [FromQuery] PaginationResourceParameters parameters_pagination,
            [FromHeader(Name = "Accept")] string mediaType 
        //[FromQuery] string keyword,
        //string rating //lessThan, largerThan, equalTo
        ) // FromQuery负责接收url的参数, FromBody负责接收请求主体
        {
            if(!MediaTypeHeaderValue
                .TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if(!_propertyMappingService.IsMappingExists<TouristRouteDto, TouristRoute>(parameters.OrderBy))
            {
                return BadRequest("请输入正确的排序参数");
            }

            if (!_propertyMappingService.IsPropertiesExists<TouristRouteDto>(parameters.Fields))
            {
                return BadRequest("请输入正确的塑型参数");
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

            bool isHateoas = parsedMediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            var primaryMediaType = isHateoas
                ? parsedMediaType.SubTypeWithoutSuffix
                    .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
                : parsedMediaType.SubTypeWithoutSuffix;

            IEnumerable<object> touristRoutesDto;
            IEnumerable<ExpandoObject> shapedDtoList;

            if (primaryMediaType == "vnd.clu.touristRoute.simplify")
            {
                touristRoutesDto = _mapper
                    .Map<IEnumerable<TouristRoutesSimplifyDto>>(touristRoutesFromRepo);

                shapedDtoList = ((IEnumerable<TouristRoutesSimplifyDto>)touristRoutesDto)
                    .ShapeData(parameters.Fields);
            }
            else
            {
                touristRoutesDto = _mapper
                    .Map<IEnumerable<TouristRouteDto>>(touristRoutesFromRepo);
                shapedDtoList =
                    ((IEnumerable<TouristRouteDto>)touristRoutesDto)
                    .ShapeData(parameters.Fields);
            }

            if (isHateoas)
            {
                var linkDto = CreateLinksForTouristRouteList(parameters, parameters_pagination);

                var shapedDtoWithLinklist = shapedDtoList.Select(t =>
                {
                    var touristRouteDictionary = t as IDictionary<string, object>;
                    var links = CreateLinkForTouristRoute(
                        (Guid)touristRouteDictionary["Id"], null);
                    touristRouteDictionary.Add("links", links);
                    return touristRouteDictionary;
                });

                var result = new
                {
                    value = shapedDtoWithLinklist,
                    links = linkDto
                };

                return Ok(result);
            }

            return Ok(shapedDtoList);
        }

        // api/touristroutes/{touristRouteId}
        [HttpGet("{touristRouteId}", Name = "GetTouristRouteById")]
        public async Task<IActionResult> GetTouristRouteById(
            Guid touristRouteId,
            string fields
        )
        {
            if (!_propertyMappingService.IsPropertiesExists<TouristRouteDto>(fields))
            {
                return BadRequest("请输入正确的塑型参数");
            }
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

            //return Ok(touristRouteDto.ShapeData(fields));

            var linkDtos = CreateLinkForTouristRoute(touristRouteId, fields);

            var result = touristRouteDto.ShapeData(fields) as IDictionary<string, object>;
            result.Add("links", linkDtos);

            return Ok(result);
        }
        #endregion

        #region POST
        [HttpPost(Name = "CreateTouristRoute")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRouteRepository.AddTouristRoute(touristRouteModel);
            await _touristRouteRepository.SaveAsync();
            var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);

            var links = CreateLinkForTouristRoute(touristRouteModel.Id, null);

            var result = touristRouteToReturn.ShapeData(null) as IDictionary<string, object>;

            result.Add("links", links);

            return CreatedAtRoute(
                "GetTouristRouteById",
                new { touristRouteId = result["Id"] },
                result
            );
        }
        #endregion

        #region PUT&PATCH
        [HttpPut("{touristRouteId}", Name = "UpdateTouristRoute")]
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

        [HttpPatch("{touristRouteId}", Name = "PartiallyUpdateTouristRoute")]
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
        [HttpDelete("{touristRouteId}", Name = "DeleteTouristRoute")]
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
