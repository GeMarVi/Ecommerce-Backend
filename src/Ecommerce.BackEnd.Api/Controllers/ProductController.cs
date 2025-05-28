using Ecommerce.BackEnd.UseCases.Products;
using Microsoft.AspNetCore.Mvc;
using ROP.APIExtensions;

namespace Ecommerce.Backend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
     
        [HttpGet("v1/get-products")]
        public async Task<IActionResult> GetPaginatedProduct([FromServices] GetProducts getPaginatedProducts, 
                                                             [FromQuery] int page = 1, 
                                                             [FromQuery] int pageSize = 10)
        {
            return await getPaginatedProducts.Execute(page, pageSize).ToActionResult();
        }

        [HttpGet("v1/get-products-filtered")]
        public async Task<IActionResult> FilterProducts(
                                                         [FromServices] GetProductsByFilter getProductsByFilter,
                                                         [FromQuery] Dictionary<string, string> filters,
                                                         [FromQuery] int page = 1,
                                                         [FromQuery] int pageSize = 10)
        {
            filters.Remove("page");
            filters.Remove("pageSize");

            return await getProductsByFilter.Execute(filters, page, pageSize).ToActionResult();
        }


        [HttpGet("v1/get-products-by-name-or-model")]
        public async Task<IActionResult> FilterProducts([FromServices] GetProductByNameOrModel getProductByName,
                                                        [FromQuery] string nameOrModel)
        {
            return await getProductByName.Execute(nameOrModel).ToActionResult();
        }
    }
} 
