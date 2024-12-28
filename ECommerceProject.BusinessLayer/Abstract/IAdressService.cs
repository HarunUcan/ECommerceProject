using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.BusinessLayer.Abstract
{
    public interface IAdressService : IGenericService<Adress>
    {
        List<Adress> TGetAdressesByUserId(int userId);
    }
}
