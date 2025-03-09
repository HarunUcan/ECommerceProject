using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.PresentationLayer.ViewModels;
using ECommerceProject.DtoLayer.Dtos.CategoryDtos;
using ECommerceProject.EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ECommerceProject.DtoLayer.Dtos.ProductImageDtos;

namespace ECommerceProject.PresentationLayer.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class CategoryController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ICategoryService _categoryService;

        public CategoryController(UserManager<AppUser> userManager, ICategoryService categoryService)
        {
            _userManager = userManager;
            _categoryService = categoryService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var categories = _categoryService.TGetList();
            return View(categories);
        }

        [HttpGet]
        public IActionResult ToggleFeatured(int id)
        {
            bool response = _categoryService.TToggleFeatured(id);
            return response == true ? Json(new { success = response }) : Json(new { error = response });
        }
        [HttpGet]
        public IActionResult ToggleTopFourCategory(int id)
        {
            bool response = _categoryService.TToggleTopFourCategory(id);
            return response == true ? Json(new { success = response }) : Json(new { error = response });
        }

        [HttpGet]
        public IActionResult Create()
        {
            AdminCategoryViewModel adminCatagoryViewModel = new AdminCategoryViewModel
            {
                CategoryDto = new CategoryDto(),
                Categories = _categoryService.TGetList()
            };
            return View(adminCatagoryViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminCategoryViewModel adminCategoryViewModel)
        {
            if (ModelState.IsValid)
            {
                string? imageUrl = null;

                if (adminCategoryViewModel.MainImage != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await adminCategoryViewModel.MainImage.CopyToAsync(memoryStream);
                        imageUrl = await _categoryService.TSaveCategoryImageAsync(memoryStream.ToArray(), adminCategoryViewModel.MainImage.FileName);
                    }
                }
                    
                _categoryService.TInsert(new Category
                {
                    Name = adminCategoryViewModel.CategoryDto.Name,
                    ParentCategoryId = adminCategoryViewModel.CategoryDto.ParentCategoryId,
                    ImageUrl = imageUrl
                });
                return RedirectToAction("Index");
            }
            return View(adminCategoryViewModel);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _categoryService.TGetById(id);
            if (category == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.ParentCategoryId = category.ParentCategoryId;
            AdminCategoryEditViewModel adminCategoryViewModel = new AdminCategoryEditViewModel
            {
                CategoryEditDto = new CategoryEditDto
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    ParentCategoryId = category.ParentCategoryId
                },
                Categories = _categoryService.TGetList()
            };
            return View(adminCategoryViewModel);
        }

        [HttpPost]
        public IActionResult Edit(AdminCategoryEditViewModel adminCategoryEditViewModel)
        {
            if (ModelState.IsValid)
            {
                _categoryService.TUpdate(new Category
                {
                    CategoryId = adminCategoryEditViewModel.CategoryEditDto.CategoryId,
                    Name = adminCategoryEditViewModel.CategoryEditDto.Name,
                    ParentCategoryId = adminCategoryEditViewModel.CategoryEditDto.ParentCategoryId
                });
                return RedirectToAction("Index");
            }
            return View(adminCategoryEditViewModel);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            _categoryService.TRecursiveDeleteCategory(id);
            return RedirectToAction("Index");
        }
    }
}
