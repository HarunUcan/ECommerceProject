using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.BusinessLayer.Helpers;
using ECommerceProject.DtoLayer.Dtos.CartDtos;
using ECommerceProject.DtoLayer.Dtos.ProductDtos;
using ECommerceProject.DtoLayer.Dtos.ProductVariantDtos;
using ECommerceProject.DtoLayer.Dtos.StaticPageDtos;
using ECommerceProject.EntityLayer.Concrete;
using ECommerceProject.PresentationLayer.Models;
using ECommerceProject.PresentationLayer.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ECommerceProject.PresentationLayer.Areas.User.Controllers;

[Area("User")]
public class HomeController : BaseUserController
{
    private readonly IProductService _productService;
    private readonly IStaticPageService _staticPageService;

    public HomeController(IProductService productService, ICategoryService categoryService, ICartService cartService, UserManager<AppUser> userManager, IStaticPageService staticPageService)
        : base(categoryService, cartService, userManager)
    {
        _productService = productService;
        _staticPageService = staticPageService;
    }

    public async Task<IActionResult> Index()
    {
        var context = await BuildUserViewContextAsync();
        var featuredProducts = await _productService.TGetFeaturedProductsAsync();
        var featuredCategoryProducts = await _productService.TGetFeaturedCategoryProductsAsync();

        var homeViewModel = new HomeViewModel
        {
            FeaturedProducts = featuredProducts,
            Categories = context.Categories,
            FeaturedCategoryProducts = featuredCategoryProducts,
            Cart = context.Cart
        };
        return View(homeViewModel);
    }

    [HttpGet]
    public IActionResult HexColorTester(string hex)
    {
        hex = Regex.Replace(hex, "[^0-9A-Fa-f]", string.Empty);
        var hexCode = $"#{hex}";
        return Content(ColorHelper.GetNearestColor(hexCode));
    }

    [HttpGet]
    public async Task<IActionResult> GetPagedProductsByCategory(int page = 1, int pageSize = 10, int categoryId = 0, string[]? sizes = null, string[]? colors = null, int minPrice = 0, int maxPrice = int.MaxValue)
    {
        List<Product> products = await _productService.TGetPagedProductsByCategoryAsync(page, pageSize, categoryId, sizes, colors, minPrice, maxPrice);
        List<ProductDto> productDtos = new();

        foreach (var product in products)
        {
            decimal discountedPrice = product.Price;
            if (product.DiscountRate != null)
            {
                discountedPrice -= product.Price * product.DiscountRate.Value / 100m;
            }
            else if (product.DiscountAmount != null)
            {
                discountedPrice -= product.DiscountAmount.Value;
            }

            var groupProductsCount = product.ProductGroup?.Products.Count <= 1 ? 0 : product.ProductGroup.Products.Count - 1;
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
                MainImageUrl = product.ProductImages.FirstOrDefault(x => x.IsMain)?.Url.Replace("wwwroot", string.Empty),
                IsFeatured = product.IsFeatured,
                GroupProductsCount = groupProductsCount
            });
        }
        return Json(productDtos);
    }

    [HttpGet]
    public async Task<IActionResult> Category(string slug)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return RedirectToAction(nameof(NotFound));
            }

            var context = await BuildUserViewContextAsync();
            var currentCategory = context.Categories.FirstOrDefault(c => string.Equals(c.Slug, slug, StringComparison.OrdinalIgnoreCase))
                ?? CategoryService.TGetBySlug(slug);

            if (currentCategory == null)
            {
                return RedirectToAction(nameof(NotFound));
            }

            var homeViewModel = new HomeViewModel
            {
                Categories = context.Categories,
                CurrentCategory = currentCategory.CategoryId,
                Cart = context.Cart
            };
            return View(homeViewModel);
        }
        catch
        {
            return RedirectToAction(nameof(NotFound));
        }
    }

    [HttpGet]
    public async Task<IActionResult> ProductDetail(string slug)
    {
        try
        {
            var product = await _productService.TGetBySlugWithAllFeaturesAsync(slug);
            var context = await BuildUserViewContextAsync();

            decimal discountedPrice = product.Price;
            if (product.DiscountRate != null)
            {
                discountedPrice -= product.Price * product.DiscountRate.Value / 100m;
            }
            else if (product.DiscountAmount != null)
            {
                discountedPrice -= product.DiscountAmount.Value;
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
                MainImageUrl = product.ProductImages.FirstOrDefault(x => x.IsMain)?.Url.Replace("wwwroot", string.Empty),
                OtherImageUrls = product.ProductImages.Where(x => !x.IsMain).Select(x => x.Url.Replace("wwwroot", string.Empty)).ToArray(),
                IsFeatured = product.IsFeatured,
                GroupProducts = product.ProductGroup?.Products.Select(x => new GroupProductDto
                {
                    Name = x.Name,
                    ImageUrl = x.ProductImages.FirstOrDefault(img => img.IsMain)?.Url.Replace("wwwroot", string.Empty),
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
                Categories = context.Categories,
                Cart = context.Cart,
            };
            return View(productDetailViewModel);
        }
        catch
        {
            return RedirectToAction(nameof(NotFound));
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
        var context = await BuildUserViewContextAsync();
        return View(new HomeViewModel
        {
            Categories = context.Categories,
            Cart = context.Cart
        });
    }

    [HttpGet]
    [Route("hakkimizda")]
    public async Task<IActionResult> About() => View(await PrepareStaticPageViewModelAsync(StaticPageType.AboutUs));

    [HttpGet]
    [Route("magaza")]
    public async Task<IActionResult> Store() => View(await PrepareStaticPageViewModelAsync(StaticPageType.Store));

    [HttpGet]
    [Route("uyelik-sozlesmesi")]
    public async Task<IActionResult> MembershipAgreement() => View(await PrepareStaticPageViewModelAsync(StaticPageType.MembershipAgreement));

    [HttpGet]
    [Route("gizlilik-politikasi")]
    public async Task<IActionResult> Privacy() => View(await PrepareStaticPageViewModelAsync(StaticPageType.PrivacyPolicy));

    [HttpGet]
    [Route("mesafeli-satis-sozlesmesi")]
    public async Task<IActionResult> SalesContract() => View(await PrepareStaticPageViewModelAsync(StaticPageType.DistanceSalesAgreement));

    [HttpGet]
    [Route("kvkk-aydinlatma-metni")]
    public async Task<IActionResult> Kvkk() => View(await PrepareStaticPageViewModelAsync(StaticPageType.KVKK));

    [HttpGet]
    [Route("iade-ve-iptal-kosullari")]
    public async Task<IActionResult> ReturnAndCancellationPolicy() => View(await PrepareStaticPageViewModelAsync(StaticPageType.ReturnAndRefundPolicy));

    [HttpGet]
    [Route("cerez-politikasi")]
    public async Task<IActionResult> CookiePolicy() => View(await PrepareStaticPageViewModelAsync(StaticPageType.CookiePolicy));

    [HttpGet]
    [Route("sikca-sorulan-sorular")]
    public async Task<IActionResult> FAQ() => View(await PrepareStaticPageViewModelAsync(StaticPageType.FAQ));

    [HttpGet]
    [Route("odeme-secenekleri")]
    public async Task<IActionResult> PaymentOptions() => View(await PrepareStaticPageViewModelAsync(StaticPageType.PaymentOptions));

    [HttpGet]
    [Route("siparis-takip")]
    public async Task<IActionResult> OrderTrack()
    {
        var context = await BuildUserViewContextAsync();
        return View(new HomeViewModel
        {
            Categories = context.Categories,
            Cart = context.Cart
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task<StaticPageViewModel> PrepareStaticPageViewModelAsync(StaticPageType staticPageType)
    {
        var context = await BuildUserViewContextAsync();
        StaticPageDto staticPageDto = await _staticPageService.TGetDtoByEnumTypeAsync(staticPageType);

        return new StaticPageViewModel
        {
            Categories = context.Categories,
            Cart = context.Cart,
            StaticPageDto = staticPageDto
        };
    }
}
