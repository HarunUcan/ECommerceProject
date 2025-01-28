using ECommerceProject.BusinessLayer.Abstract;
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

        public IActionResult Create()
        {
            return View();
        }
    }
}
