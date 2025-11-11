using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.DataAccessLayer.Abstract
{
    public interface ISaleDal : IGenericDal<Sale>
    {
        // Task<List<Sale>> GetListWithSaleItems(int page, int pageSize, string? searchString = null, string? sortOrder = null, string? status = null, string? paymentMethod = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<Sale>> GetListWithSaleItems();
    }
}
