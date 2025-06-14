﻿using Ecommerce.BackEnd.Data.IRepository;
using Ecommerce.BackEnd.Data.Models;
using Ecommerce.BackEnd.Shared.AuthDto;
using ROP;

namespace Ecommerce.BackEnd.UseCases.Auth
{
    public class EmailConfirm
    {
        private readonly IAuthRepository _user;
        public EmailConfirm(IAuthRepository user)
        {
            _user = user;
        }

        public async Task<Result<Unit>> Execute(CodeConfirmDto code)
        {
            return await CodeValidation(code)
                        .Bind(UserConfirmAndRevokeCode);
        }

        private async Task<Result<VerificationCode>> CodeValidation(CodeConfirmDto codeConfirm)
        {
            var code = await _user.GetVerificationCode(codeConfirm.id ,codeConfirm.code);

            if (!code.Success)
                return Result.Failure<VerificationCode>("Invalid verification code.");

            if (DateTime.UtcNow > code.Value.ExpirationTime)
            {
                return Result.Failure<VerificationCode>("Expired verification code.");
            }

            return code;
        }

        private async Task<Result<Unit>> UserConfirmAndRevokeCode(VerificationCode verificationCode)
        {
            var user = await _user.GetIdentityById(verificationCode.User_Id);
            if (!user.Success)
                return Result.Failure<Unit>(user.Errors);
            user.Value.EmailConfirmed = true;
            return await _user.ConfirmIdentityAndRevokeCode(user.Value, verificationCode);
        }
    }
}
