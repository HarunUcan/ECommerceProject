using ECommerceProject.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.BusinessLayer.Abstract
{
    public interface ICartService : IGenericService<Cart>
    {
        bool TDeleteByTempUserId(string tempUserId);
        bool TTransferCart(string tempUserId, int appUserId);
        bool TAddToCart(string? tempUserId, int userId, int productId, int quantity, ProductSize size);
        Cart TGetCart(string? tempUserId, int userId);
    }
}
