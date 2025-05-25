using Ecommerce.BackEnd.Shared.AuthDto;
using Ecommerce.BackEnd.UseCases.Auth;
using Microsoft.AspNetCore.Mvc;
using ROP.APIExtensions;

namespace Ecommerce.Backend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        [HttpPost("v1/admin-register")]
        public async Task<IActionResult> UserRegister([FromForm] RegisterDto createUserDto, Register userRegister)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await userRegister.Execute(createUserDto, "Admin").ToActionResult();
        }

        [HttpPost("v1/admin-login")]
        public async Task<IActionResult> UserLogin([FromForm] RegisterDto registerUserDto, Login userLogin)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await userLogin.Execute(registerUserDto, "Admin").ToActionResult();
        } 
        
        [HttpPost("v1/create-new-role")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto createRoleDto, CreateNewRole createNewRole)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await createNewRole.Execute(createRoleDto).ToActionResult();
        } 
    }
}
