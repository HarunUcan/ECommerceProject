using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.EntityLayer.Concrete;
using ECommerceProject.PresentationLayer.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProject.PresentationLayer.Areas.User.Controllers;

[Area("User")]
public class CartController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICartService _cartService;
    private readonly ICategoryService _categoryService;

    public CartController(ICategoryService categoryService, UserManager<AppUser> userManager, ICartService cartService)
    {
        _categoryService = categoryService;
        _userManager = userManager;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user?.Id ?? 0);

        var homeViewModel = new HomeViewModel
        {
            Categories = _categoryService.TGetList(),
            Cart = cart
        };
        return View(homeViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCartAsync(int productId, int quantity, string size)
    {
        var user = await _userManager.GetUserAsync(User);

        if (Enum.TryParse<ProductSize>(size, out var productSizeEnum))
        {
            if (user != null)
            {
                await _cartService.TAddToCartAsync(null, user.Id, productId, quantity, productSizeEnum);
            }
            else
            {
                var tempUserId = Request.Cookies["TempUserId"];
                if (tempUserId != null)
                {
                    await _cartService.TAddToCartAsync(tempUserId, 0, productId, quantity, productSizeEnum);
                }
            }
        }

        return RedirectToAction("Index", "Cart");
    }

    [HttpGet]
    public async Task<IActionResult> DeleteProductFromCart(int productId, string size)
    {
        var user = await _userManager.GetUserAsync(User);
        if (Enum.TryParse<ProductSize>(size, out var productSizeEnum))
        {
            if (user != null)
            {
                await _cartService.TDeleteCartItemAsync(null, user.Id, productId, productSizeEnum);
            }
            else
            {
                var tempUserId = Request.Cookies["TempUserId"];
                if (tempUserId != null)
                {
                    await _cartService.TDeleteCartItemAsync(tempUserId, 0, productId, productSizeEnum);
                }
            }
        }
        return RedirectToAction("Index", "Cart");
    }

    [HttpPost]
    public async Task<IActionResult> ApplyCoupon(string couponCode)
    {
        var user = await _userManager.GetUserAsync(User);

        try
        {
            if (user != null)
            {
                await _cartService.TApplyCoupon(null, user.Id, couponCode);
            }
            else
            {
                var tempUserId = Request.Cookies["TempUserId"];
                if (tempUserId != null)
                {
                    await _cartService.TApplyCoupon(tempUserId, 0, couponCode);
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

        var updatedCart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user?.Id ?? 0);
        return View("Index", new HomeViewModel
        {
            Categories = _categoryService.TGetList(),
            Cart = updatedCart
        });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveCouponFromCart(string couponCode)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user != null)
        {
            await _cartService.TRemoveCouponFromCart(null, user.Id, couponCode);
        }
        else
        {
            var tempUserId = Request.Cookies["TempUserId"];
            if (tempUserId != null)
            {
                await _cartService.TRemoveCouponFromCart(tempUserId, 0, couponCode);
            }
        }
        return RedirectToAction("Index", "Cart");
    }
}
