using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.DtoLayer.Dtos.ProductImageDtos;
using ECommerceProject.EntityLayer.Concrete;
using ECommerceProject.PresentationLayer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ECommerceProject.PresentationLayer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class ProductGroupController : Controller
    {
        private readonly IProductGroupService _productGroupService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IProductImageService _productImageService;

        public ProductGroupController(IProductGroupService productGroupService, ICategoryService categoryService, IProductService productService, IProductImageService productImageService)
        {
            _productGroupService = productGroupService;
            _categoryService = categoryService;
            _productService = productService;
            _productImageService = productImageService;
        }

        public IActionResult Index()
        {
            List<ProductGroup> productGroups = _productGroupService.TGetAllProductGroupsWithProducts();
            return View(productGroups);
        }

        [HttpGet]
        public IActionResult CreateProductGroup()
        {
            AdminProductGroupViewModel model = new AdminProductGroupViewModel
            {
                Categories = _categoryService.TGetList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductGroup(AdminProductGroupViewModel model)
        {
            if (model.Products != null)
            {
                for (int i = 0; i < model.Products.Count; i++)
                {
                    var viewModelProduct = model.Products[i];
                    var hasVariants = viewModelProduct.Variants != null && viewModelProduct.Variants.Any();
                    viewModelProduct.HasSizeOptions = hasVariants;

                    if (hasVariants)
                    {
                        continue;
                    }

                    viewModelProduct.Variants = null;
                    var variantKeys = ModelState.Keys.Where(k => k.StartsWith($"Products[{i}].Variants"));
                    foreach (var key in variantKeys)
                    {
                        ModelState[key].Errors.Clear();
                        ModelState[key].ValidationState = ModelValidationState.Valid;
                    }
                }
            }

            if (ModelState.IsValid)
            {
                // Burada ProductGroupService sınıfına ProductGroup return eden bir Insert metodu yazılacak
                // çünkü ProductGroup oluşturulduktan sonra Id'si lazım olacak
                _productGroupService.TInsert(new ProductGroup
                {
                    Name = model.GroupName,
                    Description = model.GroupDescription,
                    CategoryId = model.GroupCategoryId,
                });
                var productGroup = _productGroupService.TGetList().LastOrDefault();


                List<Product> products = new List<Product>();
                foreach (var product in model.Products)
                {
                    List<ProductImageDto> productImageDtos = new List<ProductImageDto>();
                    if (product.MainImage != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            product.MainImage.CopyTo(memoryStream);
                            productImageDtos.Add(new ProductImageDto
                            {
                                ImageData = memoryStream.ToArray(),
                                ImageName = product.MainImage.FileName,
                                IsMain = true
                            });
                        }
                    }

                    if (product.AdditionalImages != null)
                    {
                        foreach (var image in product.AdditionalImages)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                image.CopyTo(memoryStream);
                                productImageDtos.Add(new ProductImageDto
                                {
                                    ImageData = memoryStream.ToArray(),
                                    ImageName = image.FileName,
                                    IsMain = false
                                });
                            }
                        }
                    }

                    List<ProductVariant> productVariants = new List<ProductVariant>();
                    if (product.HasSizeOptions && product.Variants != null)
                    {
                        foreach (var variant in product.Variants)
                        {
                            productVariants.Add(new ProductVariant
                            {
                                Size = Enum.TryParse(variant.Size, out ProductSize size) ? size : ProductSize.NOSIZE,
                                Stock = variant.Stock
                            });
                        }
                    }

                    List<ProductImage> productImages = await _productImageService.SaveProductImageAsync(productImageDtos);


                    List<BasketItem> basketItems = new List<BasketItem>();

                    decimal productPrice = product.UseGroupPrice != null && product.UseGroupPrice == true ? model.GroupPrice ?? 0 : product.Price ?? 0;
                    string productDescription = product.UseGroupDescription != null && product.UseGroupDescription == true ? model.GroupDescription : product.Description;
                    var computedStock = product.HasSizeOptions
                        ? productVariants.Sum(x => x.Stock)
                        : (product.Stock ?? 0);

                    products.Add(new Product
                    {
                        Name = $"{model.GroupName} - {product.ProductName}",
                        Description = productDescription,
                        Color = product.Color,
                        Price = productPrice,
                        UniqueCode = $"{DateTime.Now.Year.ToString().Substring(2)}{DateTime.Now.Month}{DateTime.Now.Day}{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}",
                        Stock = computedStock,
                        HasSizeOptions = product.HasSizeOptions,
                        ProductGroupId = productGroup.ProductGroupId,
                        CategoryId = model.GroupCategoryId,
                        ProductVariants = product.HasSizeOptions ? productVariants : new List<ProductVariant>(),
                        ProductImages = productImages,
                        BasketItems = basketItems
                    });
                }
                await _productService.TInsertRange(products);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            _productGroupService.TDeleteWithProducts(id);
            return RedirectToAction("Index");
        }

    }
}
