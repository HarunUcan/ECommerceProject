using ECommerceProject.BusinessLayer.Abstract;
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

        public ProductController(UserManager<AppUser> userManager, IProductService productService)
        {
            _userManager = userManager;
            _productService = productService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                List<ProductVariant> productVariants = new List<ProductVariant>();
                foreach (var item in model.AdminProductSizeStocks)
                {
                    ProductSize productSize;
                    switch(item.Size)
                    {
                        case "S":
                            productSize = ProductSize.S;
                            break;
                        case "M":
                            productSize = ProductSize.M;
                            break;
                        case "L":
                            productSize = ProductSize.L;
                            break;
                        case "XL":
                            productSize = ProductSize.XL;
                            break;
                        case "XXL":
                            productSize = ProductSize.XXL;
                            break;
                        default:
                            productSize = ProductSize.XS;
                            break;
                    }
                    productVariants.Add(new ProductVariant
                    {
                        Size = productSize,
                        Stock = item.Stock
                    });
                }

                List<ProductImage> productImages = new List<ProductImage>();
                if (model.MainImage != null)
                {
                    string mainImagePath = Path.Combine("wwwroot/uploads", model.MainImage.FileName);
                    using (var stream = new FileStream(mainImagePath, FileMode.Create))
                    {
                        await model.MainImage.CopyToAsync(stream);
                        productImages.Add(new ProductImage
                        {
                            Url = mainImagePath,
                            IsMain = true
                        });
                    }
                }

                if (model.AdditionalImages != null)
                {
                    foreach (var image in model.AdditionalImages)
                    {
                        string imagePath = Path.Combine("wwwroot/uploads", image.FileName);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                            productImages.Add(new ProductImage
                            {
                                Url = imagePath
                            });
                        }
                    }
                }

                List<CartItem> cartItems = new List<CartItem>();
                var product = new Product
                {
                    Name = model.ProductName,
                    Description = model.Description,
                    Price = model.Price,
                    UniqueCode = $"{DateTime.Now.Year.ToString().Substring(2)}{DateTime.Now.Month}{DateTime.Now.Day}{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}",
                    Stock = model.AdminProductSizeStocks.Sum(x => x.Stock),//Toplam stok
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
    }
}
