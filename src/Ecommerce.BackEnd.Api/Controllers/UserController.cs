using Ecommerce.BackEnd.Shared.AuthDto;
using Ecommerce.BackEnd.UseCases.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ROP.APIExtensions;

namespace Ecommerce.Backend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost("v1/user-register")]
        public async Task<IActionResult> UserRegister([FromForm] RegisterDto createUserDto, Register userRegister) 
        {
            if (!ModelState.IsValid)
               return BadRequest(ModelState);
            
            return await userRegister.Execute(createUserDto, "User").ToActionResult();             
        }

        [HttpPost("v1/user-login")]
        public async Task<IActionResult> UserLogin([FromForm] RegisterDto registerUserDto, Login userLogin)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await userLogin.Execute(registerUserDto, "User").ToActionResult();
        }

        [HttpPost("v1/user-confirm")]
        public async Task<IActionResult> UserConfirm([FromForm] CodeConfirmDto codeConfirm, EmailConfirm emailConfirm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await emailConfirm.Execute(codeConfirm).ToActionResult();
        }
        
        [HttpPost("v1/get-new-verification-code/{id}")]
        public async Task<IActionResult> GetNewVerificationCode(string id, NewVerificationCode newVerificationCode)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await newVerificationCode.Execute(id).ToActionResult();
        } 
        
        [HttpPost("v1/get-new-tokens")]
        public async Task<IActionResult> GetNewTokens([FromBody] TokensRequestDto tokensRequestDto, GetNewTokens getNewTokens)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await getNewTokens.Execute(tokensRequestDto).ToActionResult();
        } 
        
        [HttpPost("v1/user-log-out")]
        public async Task<IActionResult> UserLogout([FromBody] LogoutDto userLogoutDto, Logout userLogout)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await userLogout.Execute(userLogoutDto.RefreshToken).ToActionResult();
        }

        [HttpPost("v1/delete-user")]
        [Authorize(Roles = "User")]
        [InjectUserId]
        public async Task<IActionResult> CreateRole(DeleteUser deleteUser)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await deleteUser.Execute(userId!).ToActionResult();
        }

        [HttpPost("v1/test-auth-user")]
        [Authorize(Roles = "User")]
        public IActionResult TestAuthUser()
        {
            return NoContent();
        }
        
        [HttpPost("v1/test-auth-admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult TestAuthAdmin()
        {
            return NoContent();
        }
    }
}
