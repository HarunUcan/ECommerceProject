using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.BusinessLayer.Helpers;
using ECommerceProject.DtoLayer.Dtos.ProductDtos;
using ECommerceProject.DtoLayer.Dtos.ProductImageDtos;
using ECommerceProject.EntityLayer.Concrete;
using ECommerceProject.PresentationLayer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace ECommerceProject.PresentationLayer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class ProductController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IProductService _productService;
        private readonly IProductImageService _productImageService;
        private readonly ICategoryService _categoryService;
        private readonly IProductGroupService _productGroupService;

        public ProductController(UserManager<AppUser> userManager, IProductService productService, IProductImageService productImageService, ICategoryService categoryService, IProductGroupService productGroupService)
        {
            _userManager = userManager;
            _productService = productService;
            _productImageService = productImageService;
            _categoryService = categoryService;
            _productGroupService = productGroupService;
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
                var discountedPrice = product.DiscountRate != null ? product.Price - (product.Price * (decimal)product.DiscountRate / 100) 
                    : product.DiscountAmount != null ? product.Price - (decimal)product.DiscountAmount 
                    : product.Price;

                productDtos.Add(new ProductDto
                {
                    Id = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    DiscountedPrice = discountedPrice,
                    Stock = product.Stock,
                    CategoryName = product.Category.Name,
                    MainImageUrl = product.ProductImages.FirstOrDefault(x => x.IsMain)?.Url.Replace("wwwroot", ""),
                    IsFeatured = product.IsFeatured,
                    Slug = product.Slug
                });
            }
            return Json(productDtos);
        }

        [HttpGet]
        public IActionResult ToggleFeatured(int id)
        {
            bool response = _productService.TToggleFeatured(id);
            return response == true ? Json(new { success = response }) : Json(new { error = response });
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

                List<AdminProductSizesViewModel> adminProductSizesViewModel;
                List<ProductVariant> productVariants = new List<ProductVariant>();
                if (model.Variants == null)
                {
                    adminProductSizesViewModel = new List<AdminProductSizesViewModel>();
                }
                else
                {
                    adminProductSizesViewModel = model.Variants;
                    foreach (var variant in adminProductSizesViewModel)
                    {

                        productVariants.Add(new ProductVariant
                        {
                            //Color = variant.Color,
                            Size = Enum.TryParse(variant.Size, out ProductSize size) ? size : ProductSize.NOSIZE,
                            Stock = variant.Stock
                        });

                    }
                }

                List<ProductImage> productImages = await _productImageService.SaveProductImageAsync(imageDtos);

                var nearestColor = ColorHelper.GetNearestColor(model.Color);

                List<CartItem> cartItems = new List<CartItem>();
                var product = new Product
                {
                    Name = model.ProductName,
                    Description = model.Description,
                    Color = model.Color,
                    NearestColor = nearestColor,
                    Price = model.Price ?? 0,
                    UniqueCode = $"{DateTime.Now.Year.ToString().Substring(2)}{DateTime.Now.Month}{DateTime.Now.Day}{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}",
                    //Stock = adminProductSizesViewModel.Sum(x => x.Sizes.Sum(y => y.Stock)),
                    CategoryId = model.CategoryId,
                    ProductVariants = productVariants,
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

        [HttpPost]
        public IActionResult ApplyDiscount(int productId, string discountType, decimal discountValue)
        {
            var product = _productService.TGetById(productId);
            if (product != null)
            {
                if (discountType == "Percentage")
                {
                    product.DiscountRate = discountValue;
                    product.DiscountAmount = null;
                }
                else if (discountType == "Amount")
                {
                    product.DiscountAmount = discountValue;
                    product.DiscountRate = null;
                }
                else
                {
                    return Json(new { error = "Invalid discount type" });
                }

                _productService.TUpdate(product);
                return Json(new { success = true });
            }
            return Json(new { error = "Product not found" }
            );
        }

        [HttpPost]
        public IActionResult RemoveDiscount(int productId)
        {
            var product = _productService.TGetById(productId);
            if (product != null)
            {
                product.DiscountRate = null;
                product.DiscountAmount = null;
                _productService.TUpdate(product);
                return Json(new { success = true });
            }
            return Json(new { error = "Product not found" }
            );
        }

    }
}
