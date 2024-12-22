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
    public class ProductVariantManager : IProductVariantService
    {
        private readonly IProductVariantDal _productVariantDal;

        public ProductVariantManager(IProductVariantDal productVariantDal)
        {
            _productVariantDal = productVariantDal;
        }

        public void TDelete(ProductVariant t)
        {
            _productVariantDal.Delete(t);
        }

        public ProductVariant TGetById(int id)
        {
            return _productVariantDal.GetById(id);
        }

        public List<ProductVariant> TGetList()
        {
            return _productVariantDal.GetList();
        }

        public void TInsert(ProductVariant t)
        {
            _productVariantDal.Insert(t);
        }

        public void TUpdate(ProductVariant t)
        {
            _productVariantDal.Update(t);
        }
    }
}
