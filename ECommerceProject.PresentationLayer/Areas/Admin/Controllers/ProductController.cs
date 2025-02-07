using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.DtoLayer.Dtos.ProductDtos;
using ECommerceProject.DtoLayer.Dtos.ProductImageDtos;
using ECommerceProject.EntityLayer.Concrete;
using ECommerceProject.PresentationLayer.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace ECommerceProject.PresentationLayer.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class ProductController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IProductService _productService;
        private readonly IProductImageService _productImageService;
        private readonly ICategoryService _categoryService;

        public ProductController(UserManager<AppUser> userManager, IProductService productService, IProductImageService productImageService, ICategoryService categoryService)
        {
            _userManager = userManager;
            _productService = productService;
            _productImageService = productImageService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            //List<Product> products = await _productService.TGetAllProductsWithCategoriesImagesAsync();
            //List<ProductDto> productDtos = new List<ProductDto>();

            //foreach (var product in products)
            //{
            //    productDtos.Add(new ProductDto
            //    {
            //        Id = product.ProductId,
            //        Name = product.Name,
            //        Description = product.Description,
            //        Price = product.Price,
            //        Stock = product.Stock,
            //        CategoryName = product.Category.Name,
            //        MainImageUrl = product.ProductImages.FirstOrDefault(x => x.IsMain)?.Url.Replace("wwwroot", "")
            //    });
            //}
            //return View(productDtos);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedProducts(int page = 1, int pageSize = 10)
        {
            List<Product> products = await _productService.TGetPagedProductsAsync(page, pageSize);
            List<ProductDto> productDtos = new List<ProductDto>();

            foreach (var product in products)
            {
                productDtos.Add(new ProductDto
                {
                    Id = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    CategoryName = product.Category.Name,
                    MainImageUrl = product.ProductImages.FirstOrDefault(x => x.IsMain)?.Url.Replace("wwwroot", "")
                });
            }
            return Json(productDtos);
        }

        [HttpGet]
        public IActionResult Create()
        {
            AdminProductViewModel model = new AdminProductViewModel
            {
                Categories = _categoryService.TGetList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                List<ProductImageDto> imageDtos = new List<ProductImageDto>();

                if (model.MainImage != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.MainImage.CopyToAsync(memoryStream);
                        imageDtos.Add(new ProductImageDto
                        {
                            ImageData = memoryStream.ToArray(),
                            ImageName = model.MainImage.FileName,
                            IsMain = true
                        });
                    }
                }

                if (model.AdditionalImages != null)
                {
                    foreach (var image in model.AdditionalImages)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await image.CopyToAsync(memoryStream);
                            imageDtos.Add(new ProductImageDto
                            {
                                ImageData = memoryStream.ToArray(),
                                ImageName = image.FileName,
                                IsMain = false
                            });
                        }
                    }
                }

                List<ProductImage> productImages = await _productImageService.SaveProductImageAsync(imageDtos);

                List<CartItem> cartItems = new List<CartItem>();
                var product = new Product
                {
                    Name = model.ProductName,
                    Description = model.Description,
                    Price = model.Price,
                    UniqueCode = $"{DateTime.Now.Year.ToString().Substring(2)}{DateTime.Now.Month}{DateTime.Now.Day}{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}",
                    Stock = model.AdminProductSizeStocks.Sum(x => x.Stock),//Toplam stok
                    CategoryId = model.CategoryId,
                    ProductVariants = model.AdminProductSizeStocks
                        .Select(item => new ProductVariant
                        {
                            Size = Enum.TryParse(item.Size, out ProductSize size) ? size : ProductSize.XS,
                            Stock = item.Stock
                        }).ToList(),
                    ProductImages = productImages,
                    CartItems = cartItems
                };
                _productService.TInsert(product);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _productService.TDeleteWithImagesAsync(new Product { ProductId = id });
            return RedirectToAction("Index");
        }
    }
}
