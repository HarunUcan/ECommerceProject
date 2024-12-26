using ECommerceProject.DtoLayer.Dtos.AppUserDtos;
using ECommerceProject.EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProject.PresentationLayer.Controllers
{
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
    }
}
