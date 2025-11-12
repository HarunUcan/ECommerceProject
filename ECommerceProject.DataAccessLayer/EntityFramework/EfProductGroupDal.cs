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
    public class EfProductGroupDal : GenericRepository<ProductGroup>, IProductGroupDal
    {
        public List<ProductGroup> GetAllProductGroupsWithProducts()
        {
            using (var context = new Context())
            {
                return context.ProductGroups
            .Include(pg => pg.Products)
                .ThenInclude(p => p.ProductImages)
            .Include(pg => pg.Products)
                .ThenInclude(p => p.ProductVariants)
            .Select(pg => new ProductGroup
            {
                ProductGroupId = pg.ProductGroupId,
                Name = pg.Name,
                Products = pg.Products.Select(p => new Product
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Slug = p.Slug,
                    Stock = p.Stock,
                    Price = p.Price,
                    HasSizeOptions = p.HasSizeOptions,
                    ProductImages = p.ProductImages
                        .Where(pi => pi.IsMain) // Sadece ana resmi al
                        .ToList(),
                    ProductVariants = p.ProductVariants.ToList()
                }).ToList()
            })
            .ToList();
            }
        }

        public ProductGroup GetByIdWithProducts(int id)
        {
            using (var context = new Context())
            {
                return context.ProductGroups
            .Include(pg => pg.Products)
                .ThenInclude(p => p.ProductImages)
            .Include(pg => pg.Products)
                .ThenInclude(p => p.ProductVariants)
            .Select(pg => new ProductGroup
            {
                ProductGroupId = pg.ProductGroupId,
                Name = pg.Name,
                Products = pg.Products.Select(p => new Product
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Stock = p.Stock,
                    Price = p.Price,
                    HasSizeOptions = p.HasSizeOptions,
                    ProductImages = p.ProductImages.ToList(),
                    ProductVariants = p.ProductVariants.ToList()
                }).ToList()
            })
            .FirstOrDefault(pg => pg.ProductGroupId == id);
            }
        }

        public List<string> DeleteWithProducts(int id)
        {
            using (var context = new Context())
            {
                ProductGroup? productGroup = context.ProductGroups
                                                .Include(pg => pg.Products)
                                                    .ThenInclude(p => p.ProductImages)
                                                .FirstOrDefault(pg => pg.ProductGroupId == id);
            
                List<string> paths = new List<string>();
                List<Product> productsToDelete = productGroup.Products.ToList();
                foreach (var product in productGroup.Products)
                {
                    var productToDelete = context.Products.Find(product.ProductId);
                    List<ProductImage> productImages = context.ProductImages.Where(img => img.ProductId == productToDelete.ProductId).ToList();
                    paths.AddRange(productImages.Select(img => img.Url).ToList());
                }
                context.Products.RemoveRange(productsToDelete);
                context.SaveChanges();

                context.ProductGroups.Remove(productGroup);
                context.SaveChanges();

                return paths;
            }
        }
    }
}
