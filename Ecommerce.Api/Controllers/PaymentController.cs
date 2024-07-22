using Ecommerce.Model;
using Ecommerce.Services.IServices;
using Ecommerce.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PaymentController : ControllerBase
    {
        private readonly IPurchaseOrdersServices _purchaseOrders;
        public PaymentController(IPurchaseOrdersServices _purchase)
        {
            _purchaseOrders = _purchase;
        }

        [HttpPost("create-shipping/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateShippingInfo([FromForm] ShipmentInfoDto shipmentInfo, string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _purchaseOrders.CreateShippingInfoServices(id, shipmentInfo);

            if (!result.IsSuccess)
            {
                return StatusCode((int)result.StatusCode, result);
            }
            return Ok(result);
        }  
        
        [HttpGet("get-shipping-information/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetShippingInfo(string id)
        {
            var result = await _purchaseOrders.GetShippingInfoServices(id);

            if (!result.IsSuccess)
            {
                return StatusCode((int)result.StatusCode, result);
            }
            return Ok(result);
        } 
        
        [HttpPost("update-shipping-information/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateShippingInfo([FromForm] ShipmentInfoDto shipmentInfo, string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            } 
            
            if (id == null || id == "")
            {
                return BadRequest("Id shipment is required");
            }

            var result = await _purchaseOrders.UpdateShippingInfoServices(shipmentInfo, id);

            if (!result.IsSuccess)
            {
                return StatusCode((int)result.StatusCode, result);
            }
            return Ok();
        } 
        
        [HttpGet("get-shipping-information-by-user/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetShippingInfoByUser(string id)
        {
            var result = await _purchaseOrders.GetShippingInfoByUserServices(id);

            if (!result.IsSuccess)
            {
                return StatusCode((int)result.StatusCode, result);
            }
            return Ok(result);
        } 
        
        [HttpPost("create-purchase-order/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreatePurchaseOrder([FromBody] List<ListProductPreferenceMpDto> products, string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _purchaseOrders.CreatePurchaseOrder(products, id);

            return Ok(result);
        }
        
        [HttpGet("get-purchase-info/{id}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetPurchaseInfo(string id)
        {
            if (id == "" || id == null)
            {
                return BadRequest("id is required");
            }

            var result = await _purchaseOrders.GetPurchasesInfoServices(id);

            if (!result.IsSuccess)
            {
                return StatusCode((int)result.StatusCode, result);
            }

            return Ok(result);
        }
        
        [HttpPost("webhook")]
        public async Task<IActionResult> PaymentNotification([FromQuery(Name = "data.id")] long id, [FromQuery(Name = "type")] string type)
        {

            var result = await _purchaseOrders.GetInfoPaymentMPServices(id);

            if (!result)
            {
                return BadRequest();
            }

            return NoContent();
        }
    }
}

