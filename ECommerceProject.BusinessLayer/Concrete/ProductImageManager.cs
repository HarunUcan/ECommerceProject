using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.DataAccessLayer.Abstract;
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
        private readonly IProductImageDal _productImageDal;

        public ProductImageManager(IProductImageDal productImageDal)
        {
            _productImageDal = productImageDal;
        }

        public void TDelete(ProductImage t)
        {
            _productImageDal.Delete(t);
        }

        public ProductImage TGetById(int id)
        {
            return _productImageDal.GetById(id);
        }

        public List<ProductImage> TGetList()
        {
            return _productImageDal.GetList();
        }

        public void TInsert(ProductImage t)
        {
            _productImageDal.Insert(t);
        }

        public void TUpdate(ProductImage t)
        {
            _productImageDal.Update(t);
        }
    }
}
