using AutoMapper;
using FakeXiecheng.API.Dtos;
using FakeXiecheng.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        #region LOCAL VARIABLES

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITouristRouteRepository _touristRouteRepository;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;

        #endregion

        #region CONSTRUCTOR
        public OrdersController(
            IHttpContextAccessor httpContextAccessor,
            ITouristRouteRepository touristRouteRepository,
            IMapper mapper,
            IHttpClientFactory httpClientFactory
        )
        {
            _httpContextAccessor = httpContextAccessor;
            _touristRouteRepository = touristRouteRepository;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }
        #endregion

        #region GET

        // 获得当前用户的订单历史记录
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetOrders()
        {
            // 1.获得当前用户
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // 2.使用用户的ID来获取订单的历史记录
            var orders = await _touristRouteRepository.GetOrdersByUserId(userId);

            return Ok(_mapper.Map<IEnumerable<OrderDto>>(orders));
        }

        [HttpGet("{orderId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId)
        {
            // 1.获得当前用户
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // 2.访问数据库, 通过用户的Id和订单的Id来定向获得某一个订单的数据
            var order = await _touristRouteRepository.GetOrderById(orderId);

            return Ok(_mapper.Map<OrderDto>(order));
        }

        #endregion

        #region POST

        // 下单
        [HttpPost("{orderId}/placeOrder")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> PlaceOrder([FromRoute] Guid orderId)
        {
            // 1. 获得当前用户
            var userId = _httpContextAccessor
                .HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // 2. 开始处理支付信息
            var order = await _touristRouteRepository.GetOrderById(orderId);
            order.PaymentProcessing();
            await _touristRouteRepository.SaveAsync();

            // 3. 向第三方提交支付请求, 等待第三方响应
            var httpClient = _httpClientFactory.CreateClient();
            string url = @"https://localhost:3001/api/FakeVanderPaymentProcess?orderNumber={0}&returnFault={1}";
            var response = await httpClient.PostAsync(
                string.Format(url, order.Id, false),
                null
                );

            // 4. 提取支付结果, 以及支付信息
            bool isApproved = false;
            string transactionMetadata = "";
            if (response.IsSuccessStatusCode)
            {
                transactionMetadata = await response.Content.ReadAsStringAsync();
                var jsonObject = (JObject)JsonConvert.DeserializeObject(transactionMetadata);
                isApproved = jsonObject["approved"].Value<bool>();
            }
            // 5. 如果第三方支付成功, 完成订单
            if (isApproved)
            {
                order.PaymentApprove();
            } else
            {
                order.PaymentReject();
            }
            order.TransactionMetadata = transactionMetadata;
            await _touristRouteRepository.SaveAsync();

            return Ok(_mapper.Map<OrderDto>(order));
        }

        #endregion
    }
}
