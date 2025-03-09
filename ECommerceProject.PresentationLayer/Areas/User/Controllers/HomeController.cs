using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.PresentationLayer.Models;
using ECommerceProject.PresentationLayer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ECommerceProject.PresentationLayer.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public HomeController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var featuredProducts = await _productService.TGetFeaturedProductsAsync(); // Default olarak max 15 ürün getirir
            var featuredCategoryProducts = await _productService.TGetFeaturedCategoryProductsAsync(); // Default olarak kategori baþýna max 15 ürün getirir
            var categories = _categoryService.TGetList();
            var homeViewModel = new HomeViewModel
            {
                FeaturedProducts = featuredProducts,
                Categories = categories,
                FeaturedCategoryProducts = featuredCategoryProducts
            };
            return View(homeViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
