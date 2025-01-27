using ECommerceProject.DataAccessLayer.Concrete;
using ECommerceProject.DtoLayer.Dtos.AppUserDtos;
using ECommerceProject.EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceProject.PresentationLayer.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class CustomerProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public CustomerProfileController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            // Kullanıcının oturumu devam ettiği sırada örneğin kullanıcı silinirse cookiede oturum tutulduğu için program buraya kadar çalışıp null reference dönebilir.
            if (user == null)
            {
                // Çerezleri temizlemek için çıkış işlemi yaptırılabilir.
                return RedirectToAction("Index", "Login");
            }
            AppUserEditDto appUserEditDto = new AppUserEditDto
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
            return View(appUserEditDto);
        }

        [HttpPost]
        public async Task<IActionResult> Index(AppUserEditDto appUserEditDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                user.Name = appUserEditDto.Name;
                user.Surname = appUserEditDto.Surname;
                user.Email = appUserEditDto.Email;
                user.UserName = appUserEditDto.Email;
                user.PhoneNumber = appUserEditDto.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return View(appUserEditDto);
                }
            }
            return View(appUserEditDto);
        }

        /** /
        [Route("category-list-example")]
        [HttpGet]
        public List<string> CategoryListExample(string categoryName)
        {
            Context context = new Context();
            var woman = context.Categories
                .Include(c => c.SubCategories) // Alt kategorileri dahil et
                .FirstOrDefault(c => c.Name == categoryName);
            List<string> categoryList = new List<string>();
            if (woman != null)
            {
                Console.WriteLine($"Category: {woman.Name}");
                foreach (var subCategory in woman.SubCategories)
                {
                    categoryList.Add(subCategory.Name);
                }
            }
            return categoryList;
        }
        / **/

    }
}
