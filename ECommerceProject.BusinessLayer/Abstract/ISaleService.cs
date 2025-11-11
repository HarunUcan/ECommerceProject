using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.BusinessLayer.Abstract
{
    public interface ISaleService : IGenericService<Sale>
    {
        //Task<List<Sale>> TGetListWithSaleItems(int page, int pageSize, string? searchString = null, string? sortOrder = null, string? status = null, string? paymentMethod = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<Sale>> TGetListWithSaleItems();
    }
}
