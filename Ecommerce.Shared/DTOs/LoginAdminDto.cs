using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.DTOs
{
    public class LoginAdminDto
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo incorrecto")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*\W)(?=.*[a-z]).{10,}$",
            ErrorMessage = "La contraseña debe tener al menos una mayúscula, un dígito, un carácter especial y ser de al menos 10 caracteres de longitud.")]
        public string Password { get; set; }
    }
}
