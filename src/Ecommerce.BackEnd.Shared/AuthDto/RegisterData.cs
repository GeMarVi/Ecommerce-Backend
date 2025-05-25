namespace Ecommerce.BackEnd.Shared.AuthDto
{
    public class RegisterData<TUser, TVerificationCode>
    {
        public TUser Identity { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Role { get; set; } = "User";
        public TVerificationCode VerificationCode { get; set; } = default!;
    }
}

