using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.BusinessLayer.Helpers;
using ECommerceProject.DataAccessLayer.Abstract;
using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            t.Slug = transformToSlug(t.Name, t.ProductId);
            _productDal.Update(t);
        }

        public void TUpdate(Product t)
        {
            _productDal.Update(t);
        }

        public async Task<List<Product>> TGetPagedProductsAsync(int currentPage, int pageSize)
        {
            return await _productDal.GetPagedProductsAsync(currentPage, pageSize);
        }

        public async Task<List<Product>> TGetPagedProductsByCategoryAsync(int currentPage, int pageSize, int categoryId, string[]? sizes, string[]? colors, int minPrice = 0, int maxPrice = int.MaxValue)
        {
            return await _productDal.GetPagedProductsByCategoryAsync(currentPage, pageSize, categoryId, sizes, colors, minPrice, maxPrice);
        }

        public async Task TDeleteWithImagesAsync(Product product)
        {
            List<string> paths = await _productDal.DeleteWithImagesAsync(product);
            FileHelper.DeleteFiles(paths);
        }

        public async Task<int> TInsertRange(List<Product> products)
        {
            await _productDal.InsertRange(products);

            foreach (var product in products)
            {
                product.Slug = transformToSlug(product.Name, product.ProductId);
                _productDal.Update(product);
            }
            return products.Count;
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

        public async Task<List<Product>> TGetFeaturedProductsAsync(int maxProductCount = 15)
        {
            return await _productDal.GetFeaturedProductsAsync(maxProductCount);
        }

        public async Task<List<Product>> TGetFeaturedCategoryProductsAsync(int maxProductCountPerCategory = 15)
        {
            return await _productDal.GetFeaturedCategoryProductsAsync(maxProductCountPerCategory);
        }

        public async Task<List<Product>> TGetListByCategorySlugAsync(string slug)
        {
            return await _productDal.GetListByCategorySlugAsync(slug);
        }

        public async Task<Product> TGetBySlugWithAllFeaturesAsync(string slug)
        {
            return await _productDal.GetBySlugWithAllFeaturesAsync(slug);
        }

        private string transformToSlug(string name, int id)
        {
            // Küçük harfe çevir
            name = name.ToLower();

            // Türkçe karakterleri İngilizceye çevir
            name = name.Replace("ç", "c")
                       .Replace("ğ", "g")
                       .Replace("ı", "i")
                       .Replace("ö", "o")
                       .Replace("ş", "s")
                       .Replace("ü", "u");

            // Boşlukları ve geçersiz karakterleri "-" ile değiştir
            name = Regex.Replace(name, @"\s+", "-"); // Birden fazla boşluğu tek "-" yap
            name = Regex.Replace(name, @"[^a-z0-9\-]", ""); // Geçersiz karakterleri kaldır

            // Birden fazla gelen "-" karakterini tek "-" yap
            name = Regex.Replace(name, @"-+", "-");

            // Başta veya sonda "-" varsa temizle
            name = name.Trim('-');

            name += $"-{id}";

            return name;
        }
    }
}
