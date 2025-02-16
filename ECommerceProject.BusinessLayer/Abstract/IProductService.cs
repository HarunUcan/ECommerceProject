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
        Task<List<Product>> TGetAllProductsWithCategoriesImagesAsync();
        Task<List<Product>> TGetPagedProductsAsync(int currentPage, int pageSize);
        Task TDeleteWithImagesAsync(Product product);
        Task TDeleteWithImagesAsync(ICollection<Product> products);
        Task<int> TInsertRange(List<Product> products);
    }
}
