using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.EntityLayer.Concrete;
using ECommerceProject.PresentationLayer.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProject.PresentationLayer.Areas.User.Controllers
{
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

        public IActionResult Index()
        {
            var user = _userManager.GetUserAsync(User).Result;
            Cart cart = null;
            if (user != null)
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], user.Id);
            else
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], 0);
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

            if (user != null)
            {
                if (Enum.TryParse<ProductSize>(size, out var productSizeEnum))
                {
                    _cartService.TAddToCart(null, user.Id, productId, quantity, productSizeEnum);
                }
            }

            else
            {
                var tempUserId = Request.Cookies["tempUserId"];
                if (tempUserId != null)
                {
                    if (Enum.TryParse<ProductSize>(size, out var productSizeEnum))
                    {
                        _cartService.TAddToCart(tempUserId, 0, productId, quantity, productSizeEnum);
                    }
                }

            }
            return RedirectToAction("Index", "Cart");
        }

        [HttpGet]
        public IActionResult DeleteProductFromCart(int productId, string size)
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user != null)
            {
                _cartService.TDeleteCartItem(null, user.Id, productId, Enum.Parse<ProductSize>(size));
            }
            else
            {
                var tempUserId = Request.Cookies["tempUserId"];
                if (tempUserId != null)
                {
                    _cartService.TDeleteCartItem(tempUserId, 0, productId, Enum.Parse<ProductSize>(size));
                }
            }
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(string couponCode)
        {
            var user = await _userManager.GetUserAsync(User);
            bool isValid = false;

            if (user != null)
            {
                try
                {
                    isValid = await _cartService.TApplyCoupon(null, user.Id, couponCode);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            else
            {
                var tempUserId = Request.Cookies["tempUserId"];
                if (tempUserId != null)
                {
                    try
                    {
                        isValid = await _cartService.TApplyCoupon(tempUserId, 0, couponCode);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                    }
                }
            }


            return View("Index", new HomeViewModel
            {
                Categories = _categoryService.TGetList(),
                Cart = _cartService.TGetCart(Request.Cookies["tempUserId"], user?.Id ?? 0)
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
                var tempUserId = Request.Cookies["tempUserId"];
                if (tempUserId != null)
                {
                    await _cartService.TRemoveCouponFromCart(tempUserId, 0, couponCode);
                }
            }
            return RedirectToAction("Index", "Cart");
        }
    }
}
