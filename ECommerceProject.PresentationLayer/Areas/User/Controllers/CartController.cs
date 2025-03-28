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

        [HttpGet]
        public async Task<IActionResult> AddToCartAsync(int productId, int quantity, string size)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
                _cartService.TAddToCart(null, user.Id, productId, quantity, (ProductSize)int.Parse(size));
            
            else
            {
                var tempUserId = Request.Cookies["tempUserId"];
                if(tempUserId != null)
                    _cartService.TAddToCart(tempUserId, 0, productId, quantity, (ProductSize)int.Parse(size));
                
            }
            return Content("Sepete eklendi");
        }
    }
}
