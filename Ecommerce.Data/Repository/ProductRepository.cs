﻿using Ecommerce.Data.IRepository;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Data.Context;
using Ecommerce.Model;
using System.Linq.Expressions;

namespace Ecommerce.Data.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<(List<Product>, int)> GetProducts(int page, int pageSize)
        {
            try
            {
                var productsQuery = _db.Products
                    .Include(p => p.sizeStocks)
                    .Include(img => img.ImagesProduct.OrderBy(i => i.Images_Id));

                var totalCount = await productsQuery.CountAsync();
                var products = await productsQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (products, totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching products from the database.", ex);
            }
        }

        public async Task<List<Product>> GetProductByNameOrModel(string name)
        {
            try
            {
                var productsFound = await _db.Products
                .Where(p => p.ProductName.Contains(name) || p.Model.Contains(name))
                .Include(p => p.sizeStocks)
                .Include(p => p.ImagesProduct.OrderBy(i => i.Images_Id))
                .Take(10)
                .OrderBy(p => p.ProductName)
                .ToListAsync();

                return productsFound;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching products from the database.", ex);
            }
        }

        public async Task<(List<Product>, int)> GetProductsByFilter(Expression<Func<Product, bool>> filter, int page, int pageSize)
        {
            try
            {
                var productsQuery = _db.Products
                    .Where(filter)
                    .Include(p => p.sizeStocks)
                    .Include(img => img.ImagesProduct.OrderBy(i => i.Images_Id));

                var totalCount = await productsQuery.CountAsync();
                var products = await productsQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                return (products, totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching products from the database.", ex);
            }           
        }

        public async Task<(List<Product>, int)> GetProductsByDiscount(int page, int pageSize)
        {
            try
            {
                var productsQuery = _db.Products
                   .Where(p => p.DiscountRate > 0)
                   .Include(p => p.sizeStocks)
                   .Include(img => img.ImagesProduct.OrderBy(i => i.Images_Id));

                var totalCount = await productsQuery.CountAsync();
                var products = await productsQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (products, totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching products from the database.", ex);
            }
           
        }

        public async Task<(List<Product>, int)> GetProductsByNewProducts(int page, int pageSize)
        {
            try
            {
                var productsQuery = _db.Products
                   .Where(p => p.ProductStatus == "Nuevo Lanzamiento")
                   .Include(p => p.sizeStocks)
                   .Include(img => img.ImagesProduct.OrderBy(i => i.Images_Id));

                var totalCount = await productsQuery.CountAsync();

                var products = await productsQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (products, totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching products from the database.", ex);
            }
        }

        public async Task<Product> GetProductById(int id)
        {
            try
            {
                var product = await _db.Products
                .Include(p => p.sizeStocks)
                .Include(p => p.ImagesProduct.OrderBy(i => i.Images_Id))
                .FirstOrDefaultAsync(p => p.Product_Id == id);

                return product;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching products from the database.", ex);
            }
           
        }

        public async Task<List<Product>> GetProductsShoppingCart(List<int> ids)
        {
            try
            {
                var products = await _db.Products
                  .Where(p => ids.Contains(p.Product_Id))
                  .Include(p => p.sizeStocks)
                  .Include(img => img.ImagesProduct.OrderBy(i => i.Images_Id))
                  .ToListAsync();

                return products;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching products from the database.", ex);
            }
        }

        public async Task<List<string>> GetImagesUrl(int Id)
        {
            try
            {
                var url = await _db.ImagesProducts.Where(i => i.Product_Id == Id).Select(i => i.Url).ToListAsync();
                return url;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching products from the database.", ex);
            }
        }

        public async Task<Product> GetProductByIdWithoutImages(int id)
        {
            try
            {
                var product = await _db.Products
                .Include(p => p.sizeStocks)
                .FirstOrDefaultAsync(p => p.Product_Id == id);

                return product;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching products from the database.", ex);
            }
        }

        public async Task<SizeStock> GetProductStock(int id, double size)
        {
            try
            {
                var result = await _db.SizeStocks.Where(s => s.Product_Id == id && s.Size == size).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching products from the database.", ex);
            }  
        }

        public async Task UpdateProductStock(SizeStock sizeStock)
        {
            try
            {
                var result = await _db.SizeStocks.FirstOrDefaultAsync(s => s.SizeStock_Id == sizeStock.SizeStock_Id);
                result.Stock = sizeStock.Stock;
                _db.SizeStocks.Update(result);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching products from the database.", ex);
            }
            
        }
         
        public async Task<bool> CreateProduct(Product product, List<SizeStock> sizeStock, List<ImagesProduct> images)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                await _db.Products.AddAsync(product);
                await _db.SaveChangesAsync();

                foreach (var ss in sizeStock)
                {
                    ss.Product_Id = product.Product_Id;
                    await _db.SizeStocks.AddAsync(ss);
                }
                await _db.SaveChangesAsync();

                foreach (var i in images)
                {
                    i.Product_Id = product.Product_Id;
                    await _db.ImagesProducts.AddAsync(i);
                }
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            await _db.SaveChangesAsync();      
            return await Save();
        }

        public async Task<bool> UpdateImageProductVariant(int Id, string newUrl)
        {
            try
            {
                var imageDb = await _db.ImagesProducts.FirstOrDefaultAsync(p => p.Images_Id == Id);
                imageDb.Url = newUrl;
                _db.ImagesProducts.Update(imageDb);
                return await Save();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching products from the database.", ex);
            }
           
        }

        public async Task<bool> DeleteProduct(Product product)
        {
            try
            {
                _db.Products.Remove(product);
                return await Save();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching products from the database.", ex);
            }
           
        }

        public bool DoesProductExist(int id)
        {
            var product = _db.Products.FirstOrDefault(p => p.Product_Id == id);
            if (product == null)
            {
                return false;
            }
            return true;
        }

        public bool DoesProductExist(string name)
        {
            var product = _db.Products.FirstOrDefault(p => p.ProductName == name);
            if (product == null)
            {
                return false;
            }
            return true;
        }

        public string DoesImageExist(int id)
        {
            var image = _db.ImagesProducts.FirstOrDefault(p => p.Images_Id == id);
            if (image == null)
            {
                return string.Empty;
            }
            return image.Url;
        }

        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
