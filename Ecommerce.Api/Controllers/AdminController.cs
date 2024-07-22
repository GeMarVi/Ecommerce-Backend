using Ecommerce.Services.IServices;
using Ecommerce.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminServices _adminServices;
        private readonly ISendEmail _sendEmail;

        public AdminController(IAdminServices adminServices,                           
                              ISendEmail sendEmail) 
        {
            _adminServices = adminServices; 
            _sendEmail = sendEmail;
        }

        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] LoginAdminDto registerAdminDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _adminServices.CreateAdmin(registerAdminDto);

            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }

            return NoContent();
        }
        
        [HttpPost("login-admin")]
        public async Task<IActionResult> LoginAdmin([FromForm] LoginAdminDto loginUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _adminServices.LoginAdmin(loginUserDto);

            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }

            return Ok(response);
        }
    }
}
