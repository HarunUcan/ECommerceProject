using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.BusinessLayer.Helpers;
using ECommerceProject.DataAccessLayer.Concrete;
using ECommerceProject.DtoLayer.Dtos.ProductDtos;
using ECommerceProject.EntityLayer.Concrete;
using ECommerceProject.PresentationLayer.Models;
using ECommerceProject.PresentationLayer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.RegularExpressions;

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

        [HttpGet]
        public IActionResult HexColorTester(string hex)
        {
            hex = Regex.Replace(hex, "[^0-9A-Fa-f]", ""); // Geçersiz karakterleri temizle
            var hexCode = $"#{hex}";
            return Content(ColorHelper.GetNearestColor(hexCode));
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedProductsByCategory(int page = 1, int pageSize = 10, int categoryId = 0, string[]? sizes = null, string[]? colors = null, int minPrice = 0, int maxPrice = int.MaxValue)
        {
            List<Product> products = await _productService.TGetPagedProductsByCategoryAsync(page, pageSize, categoryId, sizes, colors, minPrice, maxPrice);
            List<ProductDto> productDtos = new List<ProductDto>();

            //// Filtreleme Testi
            //if (sizes != null && sizes.Length > 0)
            //{
            //    return Json(sizes);
            //}
            //else if (colors != null && colors.Length > 0)
            //{
            //    return Json(colors);
            //}
            //else if (minPrice > 0 && maxPrice < int.MaxValue)
            //{
            //    return Json(minPrice + " - " + maxPrice);
            //}

            foreach (var product in products)
            {
                productDtos.Add(new ProductDto
                {
                    Id = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Slug = product.Slug,
                    Price = product.Price,
                    Stock = product.Stock,
                    CategoryName = product.Category.Name,
                    MainImageUrl = product.ProductImages.FirstOrDefault(x => x.IsMain)?.Url.Replace("wwwroot", ""),
                    IsFeatured = product.IsFeatured
                });
            }
            return Json(productDtos);
        }

        [HttpGet]
        public async Task<IActionResult> CategoryAsync(string slug, string[]? sizes = null, string[]? colors = null, int? minPrice = 0, int? maxPrice = int.MaxValue)
        {
            try
            {
                var products = await _productService.TGetListByCategorySlugAsync(slug); // Kategoriyi arar, bulamazsa hata fýrlatýr
                var currentCategory = _categoryService.TGetBySlug(slug);
                var categories = _categoryService.TGetList();
                var homeViewModel = new HomeViewModel
                {
                    Categories = categories,
                    Products = products,
                    CurrentCategory = currentCategory.CategoryId
                };
                return View(homeViewModel);
            } catch {
                return Content("404, Sayfa Bulunamadý");
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> ProductDetail(string slug)
        {
            try
            {
                var product = await _productService.TGetBySlugWithAllFeaturesAsync(slug);
                var categories = _categoryService.TGetList();

                var productDto = new ProductDto
                {
                    Id = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    CategoryName = product.Category.Name,
                    MainImageUrl = product.ProductImages.FirstOrDefault(x => x.IsMain)?.Url.Replace("wwwroot", ""),
                    OtherImageUrls = product.ProductImages.Where(x => !x.IsMain).Select(x => x.Url.Replace("wwwroot", "")).ToArray(),
                    IsFeatured = product.IsFeatured,
                    GroupProducts = product.ProductGroup?.Products.Select(x => new GroupProductDto
                    {
                        ImageUrl = x.ProductImages.FirstOrDefault(x => x.IsMain)?.Url.Replace("wwwroot", ""),
                        Slug = x.Slug
                    }).ToList()
                };
                var productDetailViewModel = new ProductDetailViewModel
                {
                    ProductDto = productDto,
                    Categories = categories
                };
                return View(productDetailViewModel);
            }
            catch
            {
                return Content("404, Sayfa Bulunamadý");
            }
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
