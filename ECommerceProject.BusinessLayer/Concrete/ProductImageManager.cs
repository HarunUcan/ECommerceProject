using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.BusinessLayer.Concrete
{
    public class ProductImageManager : IProductImageService
    {
        private readonly IProductImageService _productImageService;

        public ProductImageManager(IProductImageService productImageService)
        {
            _productImageService = productImageService;
        }

        public void TDelete(ProductImage t)
        {
            _productImageService.TDelete(t);
        }

        public ProductImage TGetById(int id)
        {
            return _productImageService.TGetById(id);
        }

        public List<ProductImage> TGetList()
        {
            return _productImageService.TGetList();
        }

        public void TInsert(ProductImage t)
        {
            _productImageService.TInsert(t);
        }

        public void TUpdate(ProductImage t)
        {
            _productImageService.TUpdate(t);
        }
    }
}
