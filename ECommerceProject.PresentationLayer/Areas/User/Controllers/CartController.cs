using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.PresentationLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProject.PresentationLayer.Areas.User.Controllers
{
    [Area("User")]
    public class CartController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CartController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public IActionResult Index()
        {
            var homeViewModel = new HomeViewModel
            {
                Categories = _categoryService.TGetList()
            };
            return View(homeViewModel);
        }
    }
}
