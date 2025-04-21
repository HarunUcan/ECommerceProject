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
    public class EfStaticPageDal : GenericRepository<StaticPage>, IStaticPageDal
    {
        public async Task<StaticPage> GetByEnumTypeAsync(StaticPageType staticPageType)
        {
            using var context = new Context();
            return await context.StaticPages.FirstOrDefaultAsync(x => x.StaticPageType == staticPageType);
        }
    }
}
