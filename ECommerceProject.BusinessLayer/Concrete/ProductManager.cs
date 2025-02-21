using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.BusinessLayer.Helpers;
using ECommerceProject.DataAccessLayer.Abstract;
using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.BusinessLayer.Concrete
{
    public class ProductManager : IProductService
    {
        private readonly IProductDal _productDal;

        public ProductManager(IProductDal productDal)
        {
            _productDal = productDal;
        }

        public void TDelete(Product t)
        {
            _productDal.Delete(t);
        }

        public Product TGetById(int id)
        {
            return _productDal.GetById(id);
        }

        public List<Product> TGetList()
        {
            return _productDal.GetList();
        }

        public async Task<List<Product>> TGetAllProductsWithCategoriesImagesAsync()
        {
            return await _productDal.GetAllProductsWithCategoriesImagesAsync();
        }

        public void TInsert(Product t)
        {
            _productDal.Insert(t);
        }

        public void TUpdate(Product t)
        {
            _productDal.Update(t);
        }

        public async Task<List<Product>> TGetPagedProductsAsync(int currentPage, int pageSize)
        {
            return await _productDal.GetPagedProductsAsync(currentPage, pageSize);
        }

        public async Task TDeleteWithImagesAsync(Product product)
        {
            List<string> paths = await _productDal.DeleteWithImagesAsync(product);
            FileHelper.DeleteFiles(paths);
        }

        public async Task<int> TInsertRange(List<Product> products)
        {
            return await _productDal.InsertRange(products);
        }

        public async Task TDeleteWithImagesAsync(ICollection<Product> products)
        {
            List<string> paths = await _productDal.DeleteWithImagesAsync(products);
            FileHelper.DeleteFiles(paths);
        }

        public bool TToggleFeatured(int productId)
        {
            return _productDal.ToggleFeatured(productId);
        }
    }
}
