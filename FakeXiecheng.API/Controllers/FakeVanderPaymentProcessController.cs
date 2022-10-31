using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FakeXiecheng.API.Controllers
{
    [ApiController]
    [Route("api/FakeVanderPaymentProcess")]
    public class FakeVanderPaymentProcessController : ControllerBase
    {
        #region POST
        // 处理付款
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(
            [FromQuery] Guid orderNumber,
            [FromQuery] bool returnFault=false)
        {
            // 模拟处理过程
            await Task.Delay(3000);

            if (returnFault)
            {
                return Ok(new
                {
                    id = Guid.NewGuid(),
                    created = DateTime.UtcNow,
                    approved = false,
                    message = "Reject",
                    payment_method = "Credit Card Payment",
                    order_number = orderNumber,
                    card = new
                    {
                        card_type = "Credit Card",
                        last_four = "1234"
                    }
                });
            }

            return Ok(new
            {
                id = Guid.NewGuid(),
                created = DateTime.UtcNow,
                approved = true,
                message = "Approve",
                payment_method = "Credit Card Payment",
                order_number = orderNumber,
                card = new
                {
                    card_type = "Credit Card",
                    last_four = "1234"
                }
            });
        }
        #endregion
    }
}
