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
        public bool AddToCart(string? tempUserId, int userId, int productId, int quantity, ProductSize size)
        {
            using (Context context = new())
            {
                if(tempUserId != null)
                {
                    var cart = context.Carts.FirstOrDefault(x => x.TempUserId == tempUserId);
                    if (cart == null)
                    {
                        cart = new Cart
                        {
                            TempUserId = tempUserId
                        };
                        context.Carts.Add(cart);
                        context.SaveChanges();
                    }
                    var cartItem = context.CartItems.FirstOrDefault(x => x.CartId == cart.CartId && x.ProductId == productId && x.Size == size);
                    if (cartItem == null)
                    {
                        cartItem = new CartItem
                        {
                            CartId = cart.CartId,
                            ProductId = productId,
                            Quantity = quantity,
                            Size = size
                        };
                        context.CartItems.Add(cartItem);
                    }
                    else
                    {
                        cartItem.Quantity += quantity;
                    }
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    var cart = context.Carts.FirstOrDefault(x => x.AppUserId == userId);
                    if (cart == null)
                    {
                        cart = new Cart
                        {
                            AppUserId = userId
                        };
                        context.Carts.Add(cart);
                        context.SaveChanges();
                    }
                    var cartItem = context.CartItems.FirstOrDefault(x => x.CartId == cart.CartId && x.ProductId == productId && x.Size == size);
                    if (cartItem == null)
                    {
                        cartItem = new CartItem
                        {
                            CartId = cart.CartId,
                            ProductId = productId,
                            Quantity = quantity,
                            Size = size
                        };
                        context.CartItems.Add(cartItem);
                    }
                    else
                    {
                        cartItem.Quantity += quantity;
                    }
                    context.SaveChanges();
                    return true;
                }
            }
        }

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

        public bool DeleteCartItem(string? tempUserId, int userId, int productId, ProductSize size)
        {
            using (Context context = new())
            {
                Cart cart;
                if (tempUserId != null)
                {
                    cart = context.Carts.FirstOrDefault(x => x.TempUserId == tempUserId);
                }
                else
                {
                    cart = context.Carts.FirstOrDefault(x => x.AppUserId == userId);
                }

                if (cart != null)
                {
                    var cartItem = context.CartItems.FirstOrDefault(x => x.CartId == cart.CartId && x.ProductId == productId && x.Size == size);
                    if (cartItem != null)
                    {
                        context.CartItems.Remove(cartItem);
                        context.SaveChanges();
                        return true;
                    }
                }
                return false;
            }
        }

        public Cart GetCart(string? tempUserId, int userId)
        {
            using (Context context = new())
            {
                if (tempUserId != null)
                {
                    return context.Carts
                        .Include(c => c.CartItems)
                            .ThenInclude(ci => ci.Product) // CartItem içindeki Product'ı getir
                                .ThenInclude(p => p.ProductImages) // Product içindeki resimleri getir
                        .Include(c => c.CartItems)
                            .ThenInclude(ci => ci.Product)
                                .ThenInclude(p => p.ProductVariants) // Varyantları da getir
                        .FirstOrDefault(x => x.TempUserId == tempUserId);
                }
                else
                {
                    return context.Carts
                        .Include(c => c.CartItems)
                            .ThenInclude(ci => ci.Product)
                                .ThenInclude(p => p.ProductImages)
                        .Include(c => c.CartItems)
                            .ThenInclude(ci => ci.Product)
                                .ThenInclude(p => p.ProductVariants)
                        .FirstOrDefault(x => x.AppUserId == userId);
                }
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
