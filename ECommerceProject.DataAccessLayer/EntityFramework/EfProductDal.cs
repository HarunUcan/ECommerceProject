using ECommerceProject.DataAccessLayer.Abstract;
using ECommerceProject.DataAccessLayer.Concrete;
using ECommerceProject.DataAccessLayer.Repositories;
using ECommerceProject.EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.DataAccessLayer.EntityFramework
{
    public class EfProductDal : GenericRepository<Product>, IProductDal
    {
        public async Task<List<string>> DeleteWithImagesAsync(Product product)
        {
            using var context = new Context();
            var productToDelete = await context.Products.FindAsync(product.ProductId);
            if (productToDelete == null)
            {
                return null;
            }
            List<ProductImage> productImages = await context.ProductImages.Where(img => img.ProductId == productToDelete.ProductId).ToListAsync();
            List<string> paths = productImages.Select(img => img.Url).ToList();

            context.Products.Remove(productToDelete);
            await context.SaveChangesAsync();

            return paths;
        }

        public async Task<List<string>> DeleteWithImagesAsync(ICollection<Product> products)
        {
            using var context = new Context();
            List<string> paths = new List<string>();
            foreach (var product in products)
            {
                var productToDelete = await context.Products.FindAsync(product.ProductId);
                if (productToDelete == null)
                {
                    return null;
                }
                List<ProductImage> productImages = await context.ProductImages.Where(img => img.ProductId == productToDelete.ProductId).ToListAsync();
                paths.AddRange(productImages.Select(img => img.Url).ToList());
                context.Products.Remove(productToDelete);
            }
            await context.SaveChangesAsync();
            return paths;
        }

        public async Task<List<Product>> GetAllProductsWithCategoriesImagesAsync()
        {
            using var context = new Context();
            return await context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages.Where(img => img.IsMain)) // Sadece IsMain olanları getir
                .ToListAsync();
        }

        public async Task<List<Product>> GetFeaturedCategoryProductsAsync(int maxProductCountPerCategory = 15)
        {
            using var context = new Context();

            var featuredCategories = await context.Categories
                .Where(c => c.IsFeatured)
                .Select(c => c.CategoryId)
                .ToListAsync();

            var products = new List<Product>();

            foreach (var categoryId in featuredCategories)
            {
                var categoryProducts = await context.Products
                    .Where(p => p.CategoryId == categoryId)
                    .OrderByDescending(p => p.ProductId)
                    .Take(maxProductCountPerCategory)
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages.Where(img => img.IsMain))
                    .ToListAsync();

                products.AddRange(categoryProducts);
            }

            return products;
        }

        public async Task<List<Product>> GetFeaturedProductsAsync(int maxProductCount = 15)
        {
            using var context = new Context();
            return await context.Products
                .Where(p => p.IsFeatured)
                .OrderByDescending(p => p.ProductId)
                .Take(maxProductCount)
                .Include(p => p.Category)
                .Include(p => p.ProductImages.Where(img => img.IsMain)) // Sadece IsMain olanları getir
                .ToListAsync();
        }

        public async Task<List<Product>> GetPagedProductsAsync(int currentPage, int pageSize)
        {
            using var context = new Context();
            return await context.Products
                .OrderByDescending(p => p.ProductId)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Category)
                .Include(p => p.ProductImages.Where(img => img.IsMain)) // Sadece IsMain olanları getir
                .ToListAsync();
        }

        public async Task<int> InsertRange(List<Product> products)
        {
            using var context = new Context();
            context.Products.AddRange(products);
            return await context.SaveChangesAsync();
        }

        public bool ToggleFeatured(int productId)
        {
            using var context = new Context();
            var product = context.Products.Find(productId);

            if (product == null)
                return false;
            
            product.IsFeatured = !product.IsFeatured;
            context.SaveChanges();

            return true;
        }
    }
}
