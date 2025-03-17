using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.DataAccessLayer.Abstract
{
    public interface IProductDal : IGenericDal<Product>
    {
        Task<Product> GetByIdWithAllFeaturesAsync(int id);
        Task<List<Product>> GetAllProductsWithCategoriesImagesAsync();
        Task<List<Product>> GetPagedProductsAsync(int currentPage, int pageSize);
        Task<List<Product>> GetPagedProductsByCategoryAsync(int currentPage, int pageSize, int categoryId);
        Task<List<Product>> GetListByCategorySlugAsync(string slug);
        Task<List<Product>> GetFeaturedProductsAsync(int maxProductCount = 15);
        Task<List<Product>> GetFeaturedCategoryProductsAsync(int maxProductCountPerCategory = 15);
        Task<List<string>> DeleteWithImagesAsync(Product product);
        Task<List<string>> DeleteWithImagesAsync(ICollection<Product> products);
        Task<int> InsertRange(List<Product> products);
        bool ToggleFeatured(int productId);
    }
}
