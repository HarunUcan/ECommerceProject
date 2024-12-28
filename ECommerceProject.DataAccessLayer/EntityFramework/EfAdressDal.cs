using ECommerceProject.DataAccessLayer.Abstract;
using ECommerceProject.DataAccessLayer.Concrete;
using ECommerceProject.DataAccessLayer.Repositories;
using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.DataAccessLayer.EntityFramework
{
    public class EfAdressDal : GenericRepository<Adress>, IAdressDal
    {
        public List<Adress> GetAdressesByUserId(int userId)
        {
            using var context = new Context();
            return context.Adresses.Where(x => x.AppUserId == userId).ToList();
        }
    }
}
