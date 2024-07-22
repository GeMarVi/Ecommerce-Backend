using Microsoft.AspNetCore.Mvc;
using Ecommerce.Shared.DTOs;
using Ecommerce.Services.IServices;

namespace Ecommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly ISendEmail _sendEmail;

        public UsersController(IUserServices userServices, ISendEmail sendEmail)
        {
            _userServices = userServices;
            _sendEmail = sendEmail;
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser([FromForm] RegisterUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userServices.CreateUser(createUserDto);

            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }

            await _sendEmail.SendVerificationEmail(createUserDto.Email, response.Message);
            response.Message = "It has been created successfully, please verify your email to continue";

            return Ok(response);
        }  
        
        [HttpGet("new-verification-code/{id}")]
        public async Task<IActionResult> NewVerificationCode (string id)
        {
            var response = await _userServices.NewVerificationCode(id);

            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }

            await _sendEmail.SendVerificationEmail(response.Data.ToString(), response.Message);
            response.Data = null;
            response.Message = "A new verification code has been sent to your email";

            return Ok(response);
        }

        [HttpPost("login-user")]
        public async Task<IActionResult> LoginUser([FromForm] RegisterUserDto loginUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userServices.LoginUser(loginUserDto);

            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }

            return Ok(response);
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] UserCodeConfirmDto userCode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userServices.ConfirmEmail(userCode);

            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userServices.VerifyAndGenerateToken(tokenRequest);

            if(!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }

            return Ok(response);
        }
    }
}
   




