using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.DataAccessLayer.Abstract
{
    public interface IAdressDal : IGenericDal<Adress>
    {
        List<Adress> GetAdressesByUserId(int userId);
    }
}
