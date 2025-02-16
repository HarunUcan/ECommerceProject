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
    public class ProductGroupManager : IProductGroupService
    {
        private readonly IProductGroupDal _productGroupDal;

        public ProductGroupManager(IProductGroupDal productGroupDal)
        {
            _productGroupDal = productGroupDal;
        }

        public void TDelete(ProductGroup t)
        {
            _productGroupDal.Delete(t);
        }

        public void TDeleteWithProducts(int id)
        {
            var imagePaths =  _productGroupDal.DeleteWithProducts(id);
            FileHelper.DeleteFiles(imagePaths);
        }

        public List<ProductGroup> TGetAllProductGroupsWithProducts()
        {
            return _productGroupDal.GetAllProductGroupsWithProducts();
        }

        public ProductGroup TGetById(int id)
        {
            return _productGroupDal.GetById(id);
        }

        public ProductGroup TGetGetByIdWithProducts(int id)
        {
            return _productGroupDal.GetByIdWithProducts(id);
        }

        public List<ProductGroup> TGetList()
        {
            return _productGroupDal.GetList();
        }

        public void TInsert(ProductGroup t)
        {
            _productGroupDal.Insert(t);
        }

        public void TUpdate(ProductGroup t)
        {
            _productGroupDal.Update(t);
        }
    }
}
