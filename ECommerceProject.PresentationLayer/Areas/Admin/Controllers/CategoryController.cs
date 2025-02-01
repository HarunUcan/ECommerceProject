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
            AdminCatagoryViewModel adminCatagoryViewModel = new AdminCatagoryViewModel
            {
                NewCategory = new CategoryDto(),
                Categories = _categoryService.TGetList()
            };
            return View(adminCatagoryViewModel);
        }

        [HttpPost]
        public IActionResult Create(AdminCatagoryViewModel adminCatagoryViewModel)
        {
            if (ModelState.IsValid)
            {
                _categoryService.TInsert(new Category
                {
                    Name = adminCatagoryViewModel.NewCategory.Name,
                    ParentCategoryId = adminCatagoryViewModel.NewCategory.ParentCategoryId
                });
                return RedirectToAction("Index");
            }
            return View(adminCatagoryViewModel);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            _categoryService.TRecursivDeleteCategory(id);
            return RedirectToAction("Index");
        }
    }
}
