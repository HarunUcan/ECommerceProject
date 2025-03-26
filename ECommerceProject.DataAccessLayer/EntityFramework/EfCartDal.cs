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
    public class EfCartDal : GenericRepository<Cart>, ICartDal
    {
        public bool DeleteByTempUserId(string tempUserId)
        {
            using (Context context = new())
            {
                var cart = context.Carts.FirstOrDefault(x => x.TempUserId == tempUserId);
                if (cart != null)
                {
                    context.Carts.Remove(cart);
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public bool TransferCart(string tempUserId, int appUserId)
        {
            using (Context context = new())
            {
                var tempCart = context.Carts.Include(c => c.CartItems).FirstOrDefault(x => x.TempUserId == tempUserId);
                var userCart = context.Carts.FirstOrDefault(x => x.AppUserId == appUserId);

                if(tempCart == null || userCart == null) return false;
                
                if (tempCart != null)
                {
                    List<CartItem> tempCartItems = tempCart.CartItems.ToList();
                    if(tempCartItems != null)
                    {
                        foreach (var item in tempCartItems)
                        {
                            item.CartId = userCart.CartId;
                        }
                        context.SaveChanges();
                    }
                }
                return true;
            }
        }
    }
}
