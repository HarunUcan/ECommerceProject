using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.DataAccessLayer.Abstract
{
    public interface ICartDal : IGenericDal<Cart>
    {
        bool DeleteByTempUserId(string tempUserId);
        bool TransferCart(string tempUserId, int appUserId);
    }
}
