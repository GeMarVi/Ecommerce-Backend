using Ecommerce.Data.IRepository;
using Ecommerce.Model;
using Ecommerce.Services.IServices;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Mapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Text;

namespace Ecommerce.Services
{
    public class AdminServices : IAdminServices
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IUserRepository _userRepository;
        private readonly IGenerateJWTServices _generateJwt;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapperDto _mapper;
        public AdminServices(IAdminRepository adminRepository,
                              IUserRepository userRepository,
                              IGenerateJWTServices generateJWT,
                              UserManager<ApplicationUser> userManager,
                              IMapperDto mapper)
        {
            _adminRepository = adminRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _mapper = mapper;
            _generateJwt = generateJWT;
        }
        public async Task<ApiResponse> CreateAdmin(LoginAdminDto admin)
        {
            var exist = await _userRepository.DoesUserExistByEmail(admin.Email);

            if (exist)
            {
                return ApiResponse.Error("Email already exist", HttpStatusCode.BadRequest);
            }

            var userAdmin = _mapper.ToApplicationUser(admin);

            var adminCreated = await _adminRepository.AdminRegister(userAdmin, admin.Password);

            if (adminCreated == null)
            {
                return ApiResponse.Error("Error to Create UserAdmin", HttpStatusCode.InternalServerError);
            }

            return ApiResponse.Success("");
        }

        public async Task<ApiResponse> LoginAdmin(LoginAdminDto admin)
        {
            var exist = await _userRepository.DoesUserExistByEmail(admin.Email);

            if (!exist)
            {
                return ApiResponse.Error("Invalid Credentials", HttpStatusCode.BadRequest);
            }

            var verifyCredentials = await _userRepository.UserLogin(admin.Email, admin.Password);

            if (verifyCredentials == null)
            {
                return ApiResponse.Error("Invalid Credentials", HttpStatusCode.BadRequest);
            }

            if (!verifyCredentials.EmailConfirmed)
            {
                return ApiResponse.Error("Email needs to be confirmed", HttpStatusCode.BadRequest);
            }

            var token = await _generateJwt.GenerateTokenAsync(verifyCredentials);

            if (token == null)
            {
                return ApiResponse.Error("Error to create access token", HttpStatusCode.InternalServerError);
            }

            var user = await _userRepository.GetUserByEmail(admin.Email);
            if (user == null)
            {
                return ApiResponse.Error("Error in the server", HttpStatusCode.InternalServerError);
            }

            var result = _mapper.ToUserResponseDto(user, token);

            return ApiResponse.Success("", result);
        }
    }
}

