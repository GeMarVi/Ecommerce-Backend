namespace Ecommerce.Shared.DTOs
{
    public class UpdateUserPasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
