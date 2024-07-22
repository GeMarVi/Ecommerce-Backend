using Ecommerce.Shared.Validations;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Shared.DTOs
{
    public class UpdateImageProductVariantDto
    {
        [Required(ErrorMessage = "La imagen a editar es requerida")]
        [AllowedExtension([".jpg", ".jpeg", ".png"])]
        [MaxFileResolution(2)]
        [MaxImagesLong(1)]
        public IFormFile image { get; set; }
    }
}
