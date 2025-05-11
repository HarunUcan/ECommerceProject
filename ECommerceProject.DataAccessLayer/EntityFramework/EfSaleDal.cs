using ECommerceProject.DataAccessLayer.Abstract;
using ECommerceProject.DataAccessLayer.Concrete;
using ECommerceProject.DataAccessLayer.Repositories;
using ECommerceProject.EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.DataAccessLayer.EntityFramework
{
    public class EfSaleDal : GenericRepository<Sale>, ISaleDal
    {
        public async Task<List<Sale>> GetListWithSaleItems()
        {
            using (var context = new Context())
            {
                return await context.Sales
                    .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                    .ToListAsync();
            }
        }
    }
}
