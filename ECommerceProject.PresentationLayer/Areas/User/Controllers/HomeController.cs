using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.BusinessLayer.Helpers;
using ECommerceProject.DataAccessLayer.Concrete;
using ECommerceProject.DtoLayer.Dtos.CartDtos;
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
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user.Id);
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], 0);
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            CartDetailDto? cart = null;
            if (user != null)
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user.Id);
            else
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], 0);
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
                decimal discountedPrice = 0m;
                if (product.DiscountRate != null)
                {
                    discountedPrice = product.Price - (product.Price * product.DiscountRate.Value / 100);
                }
                else if (product.DiscountAmount != null)
                {
                    discountedPrice = product.Price - product.DiscountAmount.Value;
                }
                else
                {
                    discountedPrice = product.Price;
                }

                var groupProductsCount = product.ProductGroup?.Products.Count == null ? 0 : product.ProductGroup.Products.Count <= 1 ? 0 : product.ProductGroup.Products.Count - 1;
                var productTotalStock = product.ProductVariants.Sum(x => x.Stock);
                productDtos.Add(new ProductDto
                {
                    Id = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Slug = product.Slug,
                    Price = product.Price,
                    DiscountedPrice = discountedPrice,
                    Stock = productTotalStock,
                    CategoryName = product.Category.Name,
                    MainImageUrl = product.ProductImages.FirstOrDefault(x => x.IsMain)?.Url.Replace("wwwroot", ""),
                    IsFeatured = product.IsFeatured,
                    GroupProductsCount = groupProductsCount
                });
            }
            return Json(productDtos);
        }

        [HttpGet]
                    cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user.Id);
                    cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], 0);
        {
            try
            {
                    cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user.Id);
                    cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], 0);
                var categories = _categoryService.TGetList();

                var user = await _userManager.GetUserAsync(User);
                CartDetailDto? cart = null;

                if (user != null)
                    cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user.Id);
                else
                    cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], 0);

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
                CartDetailDto? cart = null;
                if (user != null)
                    cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user.Id);
                else
                    cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], 0);

                decimal discountedPrice = 0m;
                if (product.DiscountRate != null)
                {
                    discountedPrice = product.Price - (product.Price * product.DiscountRate.Value / 100);
                }
                else if (product.DiscountAmount != null)
                {
                    discountedPrice = product.Price - product.DiscountAmount.Value;
                }
                else
                {
                    discountedPrice = product.Price;
                }

                var productDto = new ProductDto
                {
                    Id = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    DiscountedPrice = discountedPrice,
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
        [Route("iletisim")]
        public async Task<IActionResult> Contact()
        {
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            CartDetailDto? cart = null;
            if (user != null)
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user.Id);
            else
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], 0);
            HomeViewModel homeViewModel = new HomeViewModel
            {
                Categories = categories,
                Cart = cart
            };
            return View(homeViewModel);
        }

        [HttpGet]
        [Route("hakkimizda")]
        public async Task<IActionResult> About()
        {
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            CartDetailDto? cart = null;

            if (user != null)
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user.Id);
            else
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], 0);

            StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.AboutUs);

            if (staticPage == null)
            {
                staticPage = new StaticPage
                {
                    Title = "",
                    Content = "",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };
            }

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
        [Route("magaza")]
        public async Task<IActionResult> Store()
        {
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            CartDetailDto? cart = null;

            if (user != null)
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user.Id);
            else
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], 0);

            StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.Store);

            if (staticPage == null)
            {
                staticPage = new StaticPage
                {
                    Title = "",
                    Content = "",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };
            }

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
        [Route("uyelik-sozlesmesi")]
        public async Task<IActionResult> MembershipAgreement()
        {
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            CartDetailDto? cart = null;

            if (user != null)
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user.Id);
            else
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], 0);

            StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.MembershipAgreement);

            if (staticPage == null)
            {
                staticPage = new StaticPage
                {
                    Title = "",
                    Content = "",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };
            }

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
        [Route("gizlilik-politikasi")]
        public async Task<IActionResult> Privacy()
        {
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            CartDetailDto? cart = null;

            if (user != null)
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user.Id);
            else
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], 0);

            StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.PrivacyPolicy);

            if (staticPage == null)
            {
                staticPage = new StaticPage
                {
                    Title = "",
                    Content = "",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };
            }

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
            CartDetailDto? cart = null;

            if (user != null)
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user.Id);
            else
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], 0);

            StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.DistanceSalesAgreement);

            if (staticPage == null)
            {
                staticPage = new StaticPage
                {
                    Title = "",
                    Content = "",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };
            }

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
        [Route("kvkk-aydinlatma-metni")]
        public async Task<IActionResult> Kvkk()
        {
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            CartDetailDto? cart = null;

            if (user != null)
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user.Id);
            else
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], 0);

            StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.KVKK);

            if (staticPage == null)
            {
                staticPage = new StaticPage
                {
                    Title = "",
                    Content = "",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };
            }

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
        [Route("iade-ve-iptal-kosullari")]
        public async Task<IActionResult> ReturnAndCancellationPolicy()
        {
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            CartDetailDto? cart = null;

            if (user != null)
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user.Id);
            else
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], 0);

            StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.ReturnAndRefundPolicy);

            if (staticPage == null)
            {
                staticPage = new StaticPage
                {
                    Title = "",
                    Content = "",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };
            }

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
        [Route("cerez-politikasi")]
        public async Task<IActionResult> CookiePolicy()
        {
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            CartDetailDto? cart = null;

            if (user != null)
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user.Id);
            else
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], 0);

            StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.CookiePolicy);

            if (staticPage == null)
            {
                staticPage = new StaticPage
                {
                    Title = "",
                    Content = "",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };
            }

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
        [Route("sikca-sorulan-sorular")]
        public async Task<IActionResult> FAQ()
        {
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            CartDetailDto? cart = null;

            if (user != null)
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user.Id);
            else
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], 0);

            StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.FAQ);

            if (staticPage == null)
            {
                staticPage = new StaticPage
                {
                    Title = "",
                    Content = "",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };
            }

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
        [Route("odeme-secenekleri")]
        public async Task<IActionResult> PaymentOptions()
        {
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            CartDetailDto? cart = null;

            if (user != null)
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user.Id);
            else
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], 0);

            StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.PaymentOptions);

            if (staticPage == null)
            {
                staticPage = new StaticPage
                {
                    Title = "",
                    Content = "",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };
            }

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
        [Route("siparis-takip")]
        public async Task<IActionResult> OrderTrack()
        {
            var categories = _categoryService.TGetList();
            var user = await _userManager.GetUserAsync(User);
            CartDetailDto? cart = null;
            if (user != null)
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], user.Id);
            else
                cart = await _cartService.TGetCartDetailsAsync(Request.Cookies["TempUserId"], 0);
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
