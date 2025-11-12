using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.EntityLayer.Concrete;
using ECommerceProject.PresentationLayer.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProject.PresentationLayer.Areas.User.Controllers
{
    [Area("User")]
    public class BasketController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IBasketService _basketService;
        private readonly ICategoryService _categoryService;

        public BasketController(ICategoryService categoryService, UserManager<AppUser> userManager, IBasketService basketService)
        {
            _categoryService = categoryService;
            _userManager = userManager;
            _basketService = basketService;
        }

        public IActionResult Index()
        {
            var user = _userManager.GetUserAsync(User).Result;
            Basket basket = null;
            if (user != null)
                basket = _basketService.TGetBasket(Request.Cookies["tempUserId"], user.Id);
            else
                basket = _basketService.TGetBasket(Request.Cookies["tempUserId"], 0);
            var homeViewModel = new HomeViewModel
            {
                Categories = _categoryService.TGetList(),
                Basket = basket
            };
            return View(homeViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddToBasketAsync(int productId, int quantity, string? size)
        {
            var user = await _userManager.GetUserAsync(User);
            var productSizeEnum = ProductSize.NOSIZE;

            if (!string.IsNullOrWhiteSpace(size) && Enum.TryParse<ProductSize>(size, out var parsedSize))
            {
                productSizeEnum = parsedSize;
            }

            if (user != null)
            {
                _basketService.TAddToBasket(null, user.Id, productId, quantity, productSizeEnum);
            }

            else
            {
                var tempUserId = Request.Cookies["tempUserId"];
                if (tempUserId != null)
                {
                    _basketService.TAddToBasket(tempUserId, 0, productId, quantity, productSizeEnum);
                }

            }
            return RedirectToAction("Index", "Basket");
        }

        [HttpGet]
        public IActionResult DeleteProductFromBasket(int productId, string size)
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user != null)
            {
                _basketService.TDeleteBasketItem(null, user.Id, productId, Enum.Parse<ProductSize>(size));
            }
            else
            {
                var tempUserId = Request.Cookies["tempUserId"];
                if (tempUserId != null)
                {
                    _basketService.TDeleteBasketItem(tempUserId, 0, productId, Enum.Parse<ProductSize>(size));
                }
            }
            return RedirectToAction("Index", "Basket");
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
                    isValid = await _basketService.TApplyCoupon(null, user.Id, couponCode);
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
                        isValid = await _basketService.TApplyCoupon(tempUserId, 0, couponCode);
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
                Basket = _basketService.TGetBasket(Request.Cookies["tempUserId"], user?.Id ?? 0)
            });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCouponFromBasket(string couponCode)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                await _basketService.TRemoveCouponFromBasket(null, user.Id, couponCode);
            }
            else
            {
                var tempUserId = Request.Cookies["tempUserId"];
                if (tempUserId != null)
                {
                    await _basketService.TRemoveCouponFromBasket(tempUserId, 0, couponCode);
                }
            }
            return RedirectToAction("Index", "Basket");
        }
    }
}
