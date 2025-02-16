using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.BusinessLayer.Abstract
{
    public interface IProductGroupService : IGenericService<ProductGroup>
    {
        List<ProductGroup> TGetAllProductGroupsWithProducts();
        ProductGroup TGetGetByIdWithProducts(int id);
        void TDeleteWithProducts(int id);
    }
}
