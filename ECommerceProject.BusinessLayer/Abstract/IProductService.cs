using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.BusinessLayer.Abstract
{
    public interface IProductService : IGenericService<Product>
    {
        Task<Product> TGetBySlugWithAllFeaturesAsync(string slug);
        Task<List<Product>> TGetAllProductsWithCategoriesImagesAsync();
        Task<List<Product>> TGetPagedProductsAsync(int currentPage, int pageSize);
        Task<List<Product>> TGetPagedProductsByCategoryAsync(int currentPage, int pageSize, int categoryId);
        Task<List<Product>> TGetListByCategorySlugAsync(string slug);
        Task<List<Product>> TGetFeaturedProductsAsync(int maxProductCount = 15);
        Task<List<Product>> TGetFeaturedCategoryProductsAsync(int maxProductCountPerCategory = 15);
        Task TDeleteWithImagesAsync(Product product);
        Task TDeleteWithImagesAsync(ICollection<Product> products);
        Task<int> TInsertRange(List<Product> products);
        bool TToggleFeatured(int productId);
    }
}
