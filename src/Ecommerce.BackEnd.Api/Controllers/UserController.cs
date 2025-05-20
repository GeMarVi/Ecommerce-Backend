using Ecommerce.BackEnd.Shared.AuthDto;
using Ecommerce.BackEnd.UseCases.Auth;
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
    }
}
