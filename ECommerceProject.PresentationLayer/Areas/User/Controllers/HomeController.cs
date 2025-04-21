using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.BusinessLayer.Helpers;
using ECommerceProject.DataAccessLayer.Concrete;
using ECommerceProject.DtoLayer.Dtos.ProductDtos;
using ECommerceProject.DtoLayer.Dtos.ProductVariantDtos;
using ECommerceProject.DtoLayer.Dtos.StaticPageDtos;
using ECommerceProject.EntityLayer.Concrete;
using ECommerceProject.PresentationLayer.Models;
using ECommerceProject.PresentationLayer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly ICartService _cartService;
        private readonly IStaticPageService _staticPageService;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(IProductService productService, ICategoryService categoryService, ICartService cartService, UserManager<AppUser> userManager, IStaticPageService staticPageService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _cartService = cartService;
            _userManager = userManager;
            _staticPageService = staticPageService;
        }

        public async Task<IActionResult> Index()
        {
            var featuredProducts = await _productService.TGetFeaturedProductsAsync(); // Default olarak max 15 ürün getirir
            var featuredCategoryProducts = await _productService.TGetFeaturedCategoryProductsAsync(); // Default olarak kategori baþýna max 15 ürün getirir
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            Cart cart = null;
            if (user != null)
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], user.Id);
            else
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], 0);
            var homeViewModel = new HomeViewModel
            {
                FeaturedProducts = featuredProducts,
                Categories = categories,
                FeaturedCategoryProducts = featuredCategoryProducts,
                Cart = cart
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
                var groupProductsCount = product.ProductGroup?.Products.Count == null ? 0 : product.ProductGroup.Products.Count <= 1 ? 0 : product.ProductGroup.Products.Count - 1;
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
                    IsFeatured = product.IsFeatured,
                    GroupProductsCount = groupProductsCount
                });
            }
            return Json(productDtos);
        }

        [HttpGet]
        // Bu metot layout un çalýþmasý için kullanýlýyor, listenecek ürünler GetPagedProductsByCategory metodundan ajax ile çekiliyor
        public async Task<IActionResult> CategoryAsync(string slug, string[]? sizes = null, string[]? colors = null, int? minPrice = 0, int? maxPrice = int.MaxValue)
        {
            try
            {
                var products = await _productService.TGetListByCategorySlugAsync(slug); // Kategoriyi arar, bulamazsa hata fýrlatýr
                var currentCategory = _categoryService.TGetBySlug(slug);
                var categories = _categoryService.TGetList();

                var user = await _userManager.GetUserAsync(User);
                Cart cart = null;

                if (user != null)
                    cart = _cartService.TGetCart(Request.Cookies["tempUserId"], user.Id);
                else
                    cart = _cartService.TGetCart(Request.Cookies["tempUserId"], 0);

                var homeViewModel = new HomeViewModel
                {
                    Categories = categories,
                    CurrentCategory = currentCategory.CategoryId,
                    Cart = cart
                };
                return View(homeViewModel);
            } catch {
                return RedirectToAction("NotFound", "Home");
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> ProductDetail(string slug)
        {
            try
            {
                var product = await _productService.TGetBySlugWithAllFeaturesAsync(slug);
                var categories = _categoryService.TGetList();

                var user = await _userManager.GetUserAsync(User);
                Cart cart = null;
                if (user != null)
                    cart = _cartService.TGetCart(Request.Cookies["tempUserId"], user.Id);
                else
                    cart = _cartService.TGetCart(Request.Cookies["tempUserId"], 0);

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
                        Name = x.Name,
                        ImageUrl = x.ProductImages.FirstOrDefault(x => x.IsMain)?.Url.Replace("wwwroot", ""),
                        Slug = x.Slug
                    }).ToList(),
                    ProductVariants = product.ProductVariants.Select(x => new ProductVariantDto
                    {
                        Stock = x.Stock,
                        Size = x.Size.ToString()
                    }).ToList()
                };
                var productDetailViewModel = new ProductDetailViewModel
                {
                    ProductDto = productDto,
                    Categories = categories,
                    Cart = cart,
                };
                return View(productDetailViewModel);
            }
            catch
            {
                return RedirectToAction("NotFound", "Home");
            }
        }

        [HttpGet]
        public IActionResult NotFound()
        {
            return View();
        }

        [HttpGet]
        [Route("hakkimizda")]
        public async Task<IActionResult> About()
        {
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            Cart cart = null;
            if (user != null)
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], user.Id);
            else
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], 0);
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Categories = categories,
                Cart = cart
            };
            return View(homeViewModel);
        }

        [HttpGet]
        [Route("iletisim")]
        public async Task<IActionResult> Contact()
        {
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            Cart cart = null;
            if (user != null)
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], user.Id);
            else
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], 0);
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Categories = categories,
                Cart = cart
            };
            return View(homeViewModel);
        }

        [HttpGet]
        [Route("uyelik-sozlesmesi")]
        public async Task<IActionResult> MembershipAgreement()
        {
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            Cart cart = null;
            if (user != null)
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], user.Id);
            else
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], 0);
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Categories = categories,
                Cart = cart
            };
            return View(homeViewModel);
        }

        [HttpGet]
        [Route("gizlilik-politikasi")]
        public async Task<IActionResult> Privacy()
        {
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            Cart cart = null;

            if (user != null)
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], user.Id);
            else
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], 0);

            StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.PrivacyPolicy);

            StaticPageDto staticPageDto = new StaticPageDto
            {
                Title = staticPage.Title,
                Content = staticPage.Content,
                UpdatedDate = staticPage.UpdatedDate
            };

            StaticPageViewModel staticPageViewModel = new StaticPageViewModel
            {
                Categories = categories,
                Cart = cart,
                StaticPageDto = staticPageDto
            };
            return View(staticPageViewModel);
        }

        [HttpGet]
        [Route("mesafeli-satis-sozlesmesi")]
        public async Task<IActionResult> SalesContract()
        {
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            Cart cart = null;
            if (user != null)
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], user.Id);
            else
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], 0);
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Categories = categories,
                Cart = cart
            };
            return View(homeViewModel);
        }

        [HttpGet]
        [Route("kvkk-aydinlatma-metni")]
        public async Task<IActionResult> Kvkk()
        {
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            Cart cart = null;
            if (user != null)
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], user.Id);
            else
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], 0);
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Categories = categories,
                Cart = cart
            };
            return View(homeViewModel);
        }

        [HttpGet]
        [Route("iade-ve-iptal-kosullari")]
        public async Task<IActionResult> ReturnAndCancellationPolicy()
        {
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            Cart cart = null;
            if (user != null)
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], user.Id);
            else
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], 0);
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Categories = categories,
                Cart = cart
            };
            return View(homeViewModel);
        }

        [HttpGet]
        [Route("cerez-politikasi")]
        public async Task<IActionResult> CookiePolicy()
        {
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            Cart cart = null;
            if (user != null)
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], user.Id);
            else
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], 0);
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Categories = categories,
                Cart = cart
            };
            return View(homeViewModel);
        }

        [HttpGet]
        [Route("siparis-takip")]
        public async Task<IActionResult> OrderTrack()
        {
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            Cart cart = null;
            if (user != null)
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], user.Id);
            else
                cart = _cartService.TGetCart(Request.Cookies["tempUserId"], 0);
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Categories = categories,
                Cart = cart
            };
            return View(homeViewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
