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
        public async Task<List<Product>> GetAllProductsWithCategoriesImagesAsync()
        {
            using var context = new Context();
            return await context.Products
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
    }
}
