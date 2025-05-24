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
        public async Task<IActionResult> UserRegister([FromForm] RegisterUserDto createUserDto, UserRegister userRegister) 
        { 
            return await userRegister.Execute(createUserDto).ToActionResult();             
        }

        [HttpPost("v1/user-login")]
        public async Task<IActionResult> UserLogin([FromForm] RegisterUserDto registerUserDto, UserLogin userLogin)
        {
            return await userLogin.Execute(registerUserDto).ToActionResult();
        }

        [HttpPost("v1/user-confirm")]
        public async Task<IActionResult> UserConfirm([FromForm] CodeConfirmDto codeConfirm, EmailConfirm emailConfirm)
        {
            return await emailConfirm.Execute(codeConfirm).ToActionResult();
        }
        
        [HttpPost("v1/get-new-verification-code/{id}")]
        public async Task<IActionResult> GetNewVerificationCode(string id, NewVerificationCode newVerificationCode)
        {
            return await newVerificationCode.Execute(id).ToActionResult();
        } 
        
        [HttpPost("v1/get-new-tokens")]
        public async Task<IActionResult> GetNewTokens([FromBody] TokensRequestDto tokensRequestDto, GetNewTokens getNewTokens)
        {
            return await getNewTokens.Execute(tokensRequestDto).ToActionResult();
        } 

        [HttpPost("v1/test-auth")]
        [Authorize(Roles = "User")]
        public IActionResult TestAuth()
        {
            return NoContent();
        }
    }
}
