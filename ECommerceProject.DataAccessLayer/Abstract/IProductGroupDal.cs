using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.DataAccessLayer.Abstract
{
    public interface IProductGroupDal : IGenericDal<ProductGroup>
    {
        List<ProductGroup> GetAllProductGroupsWithProducts();
        ProductGroup GetByIdWithProducts(int id);
        List<string> DeleteWithProducts(int id);
    }
}
