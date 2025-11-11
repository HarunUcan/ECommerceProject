using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.EntityLayer.Concrete;
using ECommerceProject.PresentationLayer.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProject.PresentationLayer.Areas.User.Controllers;

[Area("User")]
public class CartController : BaseUserController
{
    public CartController(ICategoryService categoryService, UserManager<AppUser> userManager, ICartService cartService)
        : base(categoryService, cartService, userManager)
    {
    }

    public async Task<IActionResult> Index()
    {
        var context = await BuildUserViewContextAsync();

        return View(new HomeViewModel
        {
            Categories = context.Categories,
            Cart = context.Cart
        });
    }

    [HttpPost]
    public async Task<IActionResult> AddToCartAsync(int productId, int quantity, string size)
    {
        var user = await UserManager.GetUserAsync(User);

        if (Enum.TryParse<ProductSize>(size, out var productSizeEnum))
        {
            if (user != null)
            {
                await CartService.TAddToCartAsync(null, user.Id, productId, quantity, productSizeEnum);
            }
            else
            {
                var tempUserId = Request.Cookies["TempUserId"];
                if (tempUserId != null)
                {
                    await CartService.TAddToCartAsync(tempUserId, 0, productId, quantity, productSizeEnum);
                }
            }
        }

        return RedirectToAction("Index", "Cart");
    }

    [HttpGet]
    public async Task<IActionResult> DeleteProductFromCart(int productId, string size)
    {
        var user = await UserManager.GetUserAsync(User);
        if (Enum.TryParse<ProductSize>(size, out var productSizeEnum))
        {
            if (user != null)
            {
                await CartService.TDeleteCartItemAsync(null, user.Id, productId, productSizeEnum);
            }
            else
            {
                var tempUserId = Request.Cookies["TempUserId"];
                if (tempUserId != null)
                {
                    await CartService.TDeleteCartItemAsync(tempUserId, 0, productId, productSizeEnum);
                }
            }
        }
        return RedirectToAction("Index", "Cart");
    }

    [HttpPost]
    public async Task<IActionResult> ApplyCoupon(string couponCode)
    {
        var user = await UserManager.GetUserAsync(User);

        try
        {
            if (user != null)
            {
                await CartService.TApplyCoupon(null, user.Id, couponCode);
            }
            else
            {
                var tempUserId = Request.Cookies["TempUserId"];
                if (tempUserId != null)
                {
                    await CartService.TApplyCoupon(tempUserId, 0, couponCode);
                }
                else
                {
                    throw new InvalidOperationException("Geçici kullanıcı bulunamadı.");
                }
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        var context = await BuildUserViewContextAsync();
        return View("Index", new HomeViewModel
        {
            Categories = context.Categories,
            Cart = context.Cart
        });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveCouponFromCart(string couponCode)
    {
        var user = await UserManager.GetUserAsync(User);

        if (user != null)
        {
            await CartService.TRemoveCouponFromCart(null, user.Id, couponCode);
        }
        else
        {
            var tempUserId = Request.Cookies["TempUserId"];
            if (tempUserId != null)
            {
                await CartService.TRemoveCouponFromCart(tempUserId, 0, couponCode);
            }
        }
        return RedirectToAction("Index", "Cart");
    }
}
