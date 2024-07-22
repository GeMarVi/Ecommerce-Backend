using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Validations;
using Ecommerce.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Ecommerce.Shared.Exeptions;

namespace Ecommerce.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductServices _productServices;
       
        public ProductsController(IProductServices productServices)
        {
            _productServices = productServices;
        }

        [AllowAnonymous]
        [HttpGet("get-products/{page:int}")]
        public async Task<IActionResult> GetProducts(int page = 1)
        {
             return await GetProductsByCriteria(() => _productServices.GetProductsServices(page, 12));           
        }

        [AllowAnonymous]
        [HttpGet("get-product-by-name/{Name}")]
        public async Task<IActionResult> GetProductByName(string Name)
        {
            try
            {
                var result = await _productServices.GetProductsByNameServices(Name);
                if (result == null)
                {
                    return NotFound(ApiResponse.Error("Product Not Fond", HttpStatusCode.NoContent));
                }
                return Ok(ApiResponse.Success("", result));
            }
            catch (ProductNotFoundException ex)
            {
                return NotFound(ApiResponse.Error("Not Products Found", HttpStatusCode.NotFound));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Error("An internal server error occurred", HttpStatusCode.InternalServerError));
            }
        }

        [AllowAnonymous]
        [HttpGet("get-product/{Id:int}")]
        public async Task<IActionResult> GetProductById(int Id)
        {
            var product = await _productServices.GetProductsByIdServices(Id);

            if (product == null)
            {
                return NotFound(ApiResponse.Error("Product Not Found", HttpStatusCode.NotFound));
            }

            return Ok(ApiResponse.Success("", product));
        }

        [AllowAnonymous]
        [HttpGet("get-products-by-gender/{gender}/{page:int}")]
        public async Task<IActionResult> GetProductsByGender(string gender, int page = 1)
        {
            return await GetProductsByCriteria(() => _productServices.GetProductsByGenderServices(gender, page, 12));
        } 
        
        [AllowAnonymous]
        [HttpGet("get-products-by-brand/{brand}/{page:int}")]
        public async Task<IActionResult> GetProductsByBrand(string brand, int page = 1)
        {
            return await GetProductsByCriteria(() => _productServices.GetProductsByBrandServices(brand, page, 12));
        } 
        
        [AllowAnonymous]
        [HttpGet("get-products-by-Type-Deport/{typeDeport}/{page:int}")]
        public async Task<IActionResult> GetProductsByTypeDeport(string typeDeport, int page = 1)
        {
            return await GetProductsByCriteria(() => _productServices.GetProductsByTypeDeportServices(typeDeport, page, 12));
        } 
        
        [AllowAnonymous]
        [HttpGet("get-products-by-discount/{page:int}")]
        public async Task<IActionResult> GetProductsByDiscount(int page = 1)
        {
            return await GetProductsByCriteria(() => _productServices.GetProductsByDiscountServices(page, 12));
        }
        
        [AllowAnonymous]
        [HttpGet("get-products-by-new-products/{page:int}")]
        public async Task<IActionResult> GetProductsByNewProducts(int page = 1)
        {
            return await GetProductsByCriteria(() => _productServices.GetProductsByNewProductsServices(page, 12));
        }
        
        [AllowAnonymous]
        [HttpGet("get-products-shopping-cart")]
        public async Task<IActionResult> GetProductsShopingCart([FromQuery] ShoppingCartDto shoppingCart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _productServices.GetProductsShoppingCart(shoppingCart.ids);

            if(result == null)
            {
                return StatusCode(500);
            }

            return Ok(ApiResponse.Success("", result)); 
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-products")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!SizeStockValidation.IsCorrect(createProductDto.Size))
            {
                return BadRequest(ApiResponse.Error("Las tallas deben ser de 2 a 13 en intervalos de .5", HttpStatusCode.BadRequest));
            }

            if (!SizeStockValidation.IsCorrect(createProductDto.Stock))
            {
                return BadRequest(ApiResponse.Error("El stock del producto no puedo ser menor o igual a 0", HttpStatusCode.BadRequest));
            }

            if (!SizeStockValidation.IsCorrect(createProductDto.Stock, createProductDto.Size))
            {
                return BadRequest(ApiResponse.Error("Las tallas y el Stock deben tener la misma longitud ya que cada talla corresponde a cada stock en el orden en el que se envían", HttpStatusCode.BadRequest));
            }

            var result = await _productServices.CreateProductServices(createProductDto);
            if (!result.IsSuccess)
            {
                return StatusCode((int)result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpPatch("update-product/{Id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int Id, [FromForm] UpdateProductDto updateProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var update = await _productServices.UpdateProductServices(updateProduct, Id);

            if(!update.IsSuccess)
            {
                return StatusCode((int)update.StatusCode, update);
            }

            return Ok(update);
        }

        [HttpDelete("delete-product/{Id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int Id)
        {
            var result = await _productServices.DeleteProductServices(Id);
            if (!result.IsSuccess)
            {
                return StatusCode((int)result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpPatch("update-images-product/{Id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateImagesProductVariant(int Id,[FromForm] UpdateImageProductVariantDto image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result =  await _productServices.UpdateImageServices(Id, image);

            if (!result.IsSuccess)
            {
                return StatusCode((int)result.StatusCode, result);
            }

            return Ok(result);
        }

        private async Task<IActionResult> GetProductsByCriteria(Func<Task<ListCompleteProductResponseDto>> getProductFunction)
        {
            try
            {
                var result = await getProductFunction();

                int totalPages = result.numberPages;
                var products = result.completeProducts;

                return Ok(ApiResponse.Success("", result));
            }
            catch (ProductNotFoundException ex)
            {
                return NotFound(ApiResponse.Error("Not Products Found", HttpStatusCode.NotFound));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Error("An internal server error occurred", HttpStatusCode.InternalServerError));
            }
        }
    }
}


