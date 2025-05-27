using Ecommerce.BackEnd.UseCases.Products;
using Microsoft.AspNetCore.Mvc;
using ROP.APIExtensions;
using static Ecommerce.BackEnd.UseCases.Products.GetProductsByFilter;

namespace Ecommerce.Backend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public ProductController()
        {
            
        }

        [HttpGet("v1/get-products")]
        public async Task<IActionResult> GetPaginatedProduct([FromServices] GetProducts getPaginatedProducts, 
                                                             [FromQuery] int page = 1, 
                                                             [FromQuery] int pageSize = 10)
        {
            return await getPaginatedProducts.Execute(page, pageSize).ToActionResult();
        }

        [HttpGet("v1/get-products-filtered")]
        public async Task<IActionResult> FilterProducts([FromServices] GetProductsByFilter getProductsByFilter,
                                                        [FromQuery] ProductFilterType filterType,
                                                        [FromQuery] string keyword,
                                                        [FromQuery] int page = 1, 
                                                        [FromQuery] int pageSize = 10)
        {
            return await getProductsByFilter.Execute(keyword, page, pageSize, filterType).ToActionResult();
        }

    }
}
