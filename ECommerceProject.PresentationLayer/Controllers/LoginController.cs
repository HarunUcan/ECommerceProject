using ECommerceProject.DtoLayer.Dtos.AppUserDtos;
using ECommerceProject.EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProject.PresentationLayer.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public LoginController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(AppUserLoginDto appUserLoginDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(appUserLoginDto.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, appUserLoginDto.Password, appUserLoginDto.RememberMe, true);
                    if (result.Succeeded)
                    {
                        if (!user.EmailConfirmed)
                            return Content("Lütfen mailinizi onaylayınız.");
                        return Content("Giriş Başarılı");
                        //return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View(appUserLoginDto);
        }
    }
}
