using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.DTOs
{

    public class RegisterUserDto
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*\W)(?=.*[a-z]).{6,}$",
            ErrorMessage = "La contraseña debe tener al menos una mayúscula, un dígito, un carácter especial y ser de al menos 6 caracteres de longitud.")]
        public string Password { get; set; }
    }
}
