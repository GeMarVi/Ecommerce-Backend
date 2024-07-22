using Ecommerce.Data.IRepository;
using Ecommerce.Model;
using Ecommerce.Services.IServices;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Mapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace Ecommerce.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;
        private readonly IGenerateJWTServices _jwtGenerator;
        private readonly IMapperDto _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserServices(IUserRepository userRepository,
                            IGenerateJWTServices jwtGenerator,
                            IMapperDto mapper,
                            UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _jwtGenerator = jwtGenerator;
            _mapper = mapper;
            _userManager = userManager;
        }
        public async Task<ApiResponse> CreateUser(RegisterUserDto createUserDto)
        {
            var exist = await _userRepository.DoesUserExistByEmail(createUserDto.Email);

            if (exist)
            {
                return ApiResponse.Error("User already exists", HttpStatusCode.BadRequest);
            }

            var userMapper = _mapper.ToApplicationUser(createUserDto);

            var verificationCode = new VerificationCode()
            {
                Code = new Random().Next(100000, 999999).ToString(),
                ExpirationTime = DateTime.UtcNow.AddMinutes(5),
            };
          
            var result = await _userRepository.UserRegister(userMapper, createUserDto.Password, verificationCode);

            if (result.Item1 == null)
            {
                return ApiResponse.Error("There was an error on the server while creating the user", HttpStatusCode.InternalServerError);
            }

            return ApiResponse.Success(result.Item2, result.Item1.Id);
        }

        public async Task<ApiResponse> LoginUser(RegisterUserDto loginUserDto)
        {
            var exist = await _userRepository.DoesUserExistByEmail(loginUserDto.Email);

            if (!exist)
            {
                return ApiResponse.Error("Invalid Payload", HttpStatusCode.BadRequest);
            }

            var verifyCredentials = await _userRepository.UserLogin(loginUserDto.Email, loginUserDto.Password);

            if (verifyCredentials == null)
            {
                return ApiResponse.Error("Invalid Credentials", HttpStatusCode.BadRequest);
            }

            if (!verifyCredentials.EmailConfirmed)
            {
                return ApiResponse.Error("Email needs to be confirmed", HttpStatusCode.BadRequest);
            }

            var token = await _jwtGenerator.GenerateTokenAsync(verifyCredentials);

            if (token == null)
            {
                return ApiResponse.Error("Error to generate access token", HttpStatusCode.InternalServerError);
            }

            var response = new UserResponseDto()
            {
                User_Id = verifyCredentials.Id,
                Email = verifyCredentials.Email,
                Token = token.Token,
                RefreshToken = token.RefreshToken
            };

            return ApiResponse.Success("Successful login", response);
        }

        public async Task<ApiResponse> VerifyAndGenerateToken(TokenRequestDto token)
        {
            var result = await _jwtGenerator.VerifyAndGenerateTokenAsync(token);

            if (!result.IsSuccess)
            {
                return ApiResponse.Error(result.Message, HttpStatusCode.BadRequest);
            }

            return ApiResponse.Success("", result);
        }

        public async Task<ApiResponse> ConfirmEmail(UserCodeConfirmDto userCode)
        {
                  
            var userDb = await _userRepository.GetUserById(userCode.user_Id);

            if (userDb == null) return ApiResponse.Error($"Unable to load user with id: {userCode.user_Id}", HttpStatusCode.NotFound);

            var userCodeDb = await _userRepository.UserVerificationCode(userCode.user_Id);

            if(userCodeDb == null || userCodeDb.Code != userCode.code)
            {
               return ApiResponse.Error("Incorrect verification code", HttpStatusCode.BadRequest);
            }

            if (DateTime.UtcNow > userCodeDb.ExpirationTime)
            {
                return ApiResponse.Error("Verification code has expired, please request another", HttpStatusCode.BadRequest);
            }

            userDb.EmailConfirmed = true;

            var result = await _userRepository.UserConfirm(userDb);

            if (!result)
            {
                await _userRepository.DeleteVerificationCode(userCodeDb);
                return ApiResponse.Error("There has been an error confirming your Email", HttpStatusCode.InternalServerError);
            }

            await _userRepository.DeleteVerificationCode(userCodeDb);
            return ApiResponse.Success("Confirm success");
        }

        public async Task<ApiResponse> NewVerificationCode(string id)
        {
            if (id == null || id == "")
            {
                return ApiResponse.Error("Id is required to continue", HttpStatusCode.BadRequest);
            }

            var verificationCode = new VerificationCode()
            {
                Code = new Random().Next(100000, 999999).ToString(),
                ExpirationTime = DateTime.UtcNow.AddMinutes(5),
                User_Id = id
            };

            var result = await _userRepository.UpdateVerificationCode(verificationCode);
            var user = await _userRepository.GetUserById(id);

            if(result == null || user == null)
            {
                ApiResponse.Error("Error to create new Verification Code", HttpStatusCode.InternalServerError);
            }

            return ApiResponse.Success(result.Code, user.Email);
        }
    }
}
