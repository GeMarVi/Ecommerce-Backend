using Ecommerce.Services.IServices;
using Ecommerce.Shared.DTOs;
using Ecommerce.Shared.Mapper;
using Ecommerce.Data.IRepository;
using Ecommerce.Model;
using System.Net;
using Ecommerce.Shared.Exeptions;
using Ecommerce.ServicesDependencies.CloudinaryImages;

namespace Ecommerce.Services
{
    public class ProductServices : IProductServices
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapperDto _mapper;

        public ProductServices(IProductRepository productoRepository, IMapperDto mapper)
        {
            _productRepository = productoRepository;
            _mapper = mapper;
        }

        public async Task<ListCompleteProductResponseDto> GetProductsServices(int page, int pageSize)
        {
            try
            {
                var (products, totalCount) = await _productRepository.GetProducts(page, pageSize);

                if (products == null || products.Count == 0)
                {
                    throw new ProductNotFoundException("No products found");
                }

                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
                var result = MapperToCompleteResponseDto(products);

                return new ListCompleteProductResponseDto { completeProducts = result, numberPages = totalPages};
            }
            catch (ProductNotFoundException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing products in the service layer.", ex);
            }
        }

        public async Task<List<CompleteProductResponseDto>> GetProductsByNameServices(string name)
        {
            try
            {
                var products = await _productRepository.GetProductByNameOrModel(name);
                if (products == null || products.Count == 0)
                {
                    throw new ProductNotFoundException("No Products Found");
                }
                var result = MapperToCompleteResponseDto(products);
                return result;
            }
            catch (ProductNotFoundException ex)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new Exception("An error occurred while processing products", ex);
            }
        }

        public async Task<ListCompleteProductResponseDto> GetProductsByGenderServices(string gender, int page, int pageSize)
        {
            try
            {
                var (products, totalCount) = await _productRepository.GetProductsByFilter(p => p.Gender == gender, page, pageSize);

                if (products == null || products.Count == 0)
                {
                    throw new ProductNotFoundException("No products found");
                }

                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

                var result = MapperToCompleteResponseDto(products);

                return new ListCompleteProductResponseDto { completeProducts = result, numberPages = totalPages };
            }
            catch (ProductNotFoundException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing products in the service layer.", ex);
            }

        }

        public async Task<ListCompleteProductResponseDto> GetProductsByBrandServices(string brand, int page, int pageSize)
        {
            try
            {
                var (products, totalCount) = await _productRepository.GetProductsByFilter(p => p.Brand == brand, page, pageSize);

                if (products == null || products.Count == 0)
                {
                    throw new ProductNotFoundException("No products found");
                }

                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

                var result = MapperToCompleteResponseDto(products);

                return new ListCompleteProductResponseDto { completeProducts = result, numberPages = totalPages };
            }
            catch (ProductNotFoundException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing products in the service layer.", ex);
            }

        }

        public async Task<ListCompleteProductResponseDto> GetProductsByTypeDeportServices(string typeDeport, int page, int pageSize)
        {
            try
            {
                var (products, totalCount) = await _productRepository.GetProductsByFilter(p => p.TypeDeport == typeDeport, page, pageSize);

                if (products == null || products.Count == 0)
                {
                    throw new ProductNotFoundException("No products found");
                }

                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

                var result = MapperToCompleteResponseDto(products);

                return new ListCompleteProductResponseDto { completeProducts = result, numberPages = totalPages };
            }
            catch (ProductNotFoundException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing products in the service layer.", ex);
            }
        }

        public async Task<ListCompleteProductResponseDto> GetProductsByDiscountServices(int page, int pageSize)
        {
            try
            {
                var (products, totalCount) = await _productRepository.GetProductsByDiscount(page, pageSize);

                if (products == null || products.Count == 0)
                {
                    throw new ProductNotFoundException("No products found");
                }

                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

                var result = MapperToCompleteResponseDto(products);

                return new ListCompleteProductResponseDto { completeProducts = result, numberPages = totalPages };
            }
            catch (ProductNotFoundException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing products in the service layer.", ex);
            }
        }

        public async Task<ListCompleteProductResponseDto> GetProductsByNewProductsServices(int page, int pageSize)
        {
            try
            {
                var (products, totalCount) = await _productRepository.GetProductsByNewProducts(page, pageSize);

                if (products == null || products.Count == 0)
                {
                    throw new ProductNotFoundException("No products found");
                }

                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

                var result = MapperToCompleteResponseDto(products);

                return new ListCompleteProductResponseDto { completeProducts = result, numberPages = totalPages };
            }
            catch (ProductNotFoundException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while processing products in the service layer.", ex);
            }
        }

        public async Task<ProductResponseDto> GetProductsByIdServices(int id)
        {
            var product = await _productRepository.GetProductById(id);

            if (product == null)
            {
                return null;
            }

            var result = _mapper.ToProductResponseDto(product);

            return result;
        }

        public async Task<List<string>> GetImagesUrlByIdServices(int id)
        {
            var product = await _productRepository.GetProductById(id);

            if (product == null)
            {
                return null;
            }

            List<string> urls = new List<string>();

            foreach (var item in product.ImagesProduct)
            {
                urls.Add(item.Url);
            }

            return urls;
        }

        public async Task<List<CompleteProductResponseDto>> GetProductsShoppingCart(List<int> ids)
        {
            var products = await _productRepository.GetProductsShoppingCart(ids);
            if(products == null)
            {
                return null;
            }

            var result = MapperToCompleteResponseDto(products);
            
            return result;
        }

        public async Task<ApiResponse> CreateProductServices(CreateProductDto createProduct)
        {
            try
            {
                CloudinaryImplementation cloudinary = new CloudinaryImplementation();
                var imagesResult = await cloudinary.UploadImages(createProduct.Images, createProduct.ProductName);

                if (!imagesResult.IsSuccess)
                {
                    throw new CloudinaryException("Error uploading images to Cloudinary.");
                }
                var products = _mapper.ToProduct(createProduct);
                products.CreationDate = DateTime.UtcNow;

                var sizeStocks = createProduct.Size.Select((size, index) => new SizeStock
                {
                    Size = size,
                    Stock = createProduct.Stock[index]
                }).ToList();

                var imagesProducts = imagesResult.ImageUrls.Select(url => new ImagesProduct
                {
                    Url = url
                }).ToList();

                bool result = await _productRepository.CreateProduct(products, sizeStocks, imagesProducts);
                if (!result)
                {
                    throw new DatabaseException("Error saving product to the database.");
                }

                return ApiResponse.Success("Product created successfully.");
            }
            catch (CloudinaryException ex)
            {
                return ApiResponse.Error(ex.Message, HttpStatusCode.InternalServerError);
            }
            catch (DatabaseException ex)
            {
                return ApiResponse.Error(ex.Message, HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                return ApiResponse.Error("An unexpected error occurred.", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse> UpdateProductServices(UpdateProductDto updateProduct, int Id)
        {
            try
            {
                var product = await _productRepository.GetProductByIdWithoutImages(Id);

                if (product == null)
                {
                    throw new ProductNotFoundException("Product No Found");
                }

                product.ProductName = updateProduct.ProductName;
                product.Description = updateProduct.Description;
                product.Brand = updateProduct.Brand;
                product.Model = updateProduct.Model;
                product.ClosureType = updateProduct.ClosureType;
                product.OuterMaterial = updateProduct.OuterMaterial;
                product.SoleMaterial = updateProduct.SoleMaterial;
                product.TypeDeport = updateProduct.TypeDeport;
                product.Gender = updateProduct.Gender;
                product.Color = updateProduct.Color;
                product.Price = updateProduct.Price;
                product.hasDiscount = updateProduct.HasDiscount;
                product.DiscountRate = updateProduct.DiscountRate;

                var sizeStocks = updateProduct.Size
                  .Zip(updateProduct.Stock, (size, stock) => new SizeStock { Size = size, Stock = stock })
                  .ToList();

                product.sizeStocks.Clear();
                product.sizeStocks.AddRange(sizeStocks);

                var result = await _productRepository.UpdateProduct(product);

                if (!result)
                {
                    throw new DatabaseException("Error updating product to the database.");
                }

                return ApiResponse.Success("Updated successfully");
            }
            catch (ProductNotFoundException ex)
            {
                return ApiResponse.Error("Product No Found", HttpStatusCode.NotFound);
                 
            }
            catch (DatabaseException ex)
            {
                return ApiResponse.Error("An unexpected error occurred.", HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse> UpdateImageServices(int Id, UpdateImageProductVariantDto image)
        {
            string imageDbUrl = _productRepository.DoesImageExist(Id);
            if (imageDbUrl == string.Empty)
            {
                return ApiResponse.Error("Not Found image", HttpStatusCode.NotFound);
            }

            CloudinaryImplementation cloudinaryImplementation = new CloudinaryImplementation();

            List<string> url = new List<string>()
            {
                imageDbUrl
            };

            bool deleteImage = await cloudinaryImplementation.DeleteImage(url);

            if (!deleteImage)
            {
                return ApiResponse.Error("Error delete image in Cloudinary", HttpStatusCode.InternalServerError);
            }

            string imageId = $"ecommerce/producto/{cloudinaryImplementation.ExtractImageId(imageDbUrl)}";

            var result = await cloudinaryImplementation.UploadImage(image.image, imageId);

            if (!result.IsSuccess)
            {
                return ApiResponse.Error("Error uploading image in Cloudinary", HttpStatusCode.InternalServerError);
            }

            var addUrlToDb = await _productRepository.UpdateImageProductVariant(Id, result.ImageUrls[0]);

            if (!addUrlToDb)
            {
                return ApiResponse.Error("Error saving image in the db", HttpStatusCode.InternalServerError);
            }

            return ApiResponse.Success("Delete successfully");
        }

        public async Task<ApiResponse> DeleteProductServices(int Id)
        {
            var productExist = _productRepository.DoesProductExist(Id);
            if (!productExist)
            {
                return ApiResponse.Error("Product no found", HttpStatusCode.NotFound);
            }

            var urls = await _productRepository.GetImagesUrl(Id);

            CloudinaryImplementation cloudinary = new CloudinaryImplementation();
            var isSuccess = await cloudinary.DeleteImage(urls);

            if (!isSuccess)
            {
                return ApiResponse.Error("Error delete images in Cloudinary", HttpStatusCode.InternalServerError);
            }

            var product = await _productRepository.GetProductById(Id);
            var deleteProduct = await _productRepository.DeleteProduct(product);

            if (!deleteProduct)
            {
                return ApiResponse.Error("Error delete images in the Database", HttpStatusCode.InternalServerError);
            }

            return ApiResponse.Success("Delete successfully");
        }
            
        private List<CompleteProductResponseDto> MapperToCompleteResponseDto(List<Product> products)
        {
            var result = new List<CompleteProductResponseDto>();

            foreach (var product in products)
            {
                var mapperProduct = _mapper.ToCompleteProductResponseDto(product);
                result.Add(mapperProduct);
            }

            return result;
        }
    }
}
