using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.PresentationLayer.ViewModels;
using ECommerceProject.DtoLayer.Dtos.CategoryDtos;
using ECommerceProject.EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Create(AdminCategoryViewModel adminCategoryViewModel)
        {
            if (ModelState.IsValid)
            {
                _categoryService.TInsert(new Category
                {
                    Name = adminCategoryViewModel.CategoryDto.Name,
                    ParentCategoryId = adminCategoryViewModel.CategoryDto.ParentCategoryId
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
