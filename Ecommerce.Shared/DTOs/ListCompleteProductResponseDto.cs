
namespace Ecommerce.Shared.DTOs
{
    public class ListCompleteProductResponseDto
    { 
         public int numberPages { get; set; }
         public List<CompleteProductResponseDto> completeProducts { get; set; }
    }
}
