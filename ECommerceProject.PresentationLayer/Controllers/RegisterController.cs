using ECommerceProject.DtoLayer.Dtos.AppUserDtos;
using ECommerceProject.EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProject.PresentationLayer.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public RegisterController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(AppUserRegisterDto appUserRegisterDto)
        {
            if (ModelState.IsValid)
            {
                Cart cart = new Cart
                {
                    UpdatedDate = DateTime.Now
                };
                AppUser appUser = new AppUser
                {
                    UserName = appUserRegisterDto.Email,
                    Name = appUserRegisterDto.Name,
                    Surname = appUserRegisterDto.Surname,
                    Email = appUserRegisterDto.Email,
                    Cart = cart
                };

                var result = await _userManager.CreateAsync(appUser, appUserRegisterDto.Password);

                if (result.Succeeded)
                {
                    //return RedirectToAction("Index", "ConfirmMail");
                    return Content("<h1>Son Bir Adım Kaldı!</h1> <p>Kaydınız başarılı bir şekilde gerçekleşti. Hesabınızı aktifleştirmek için mail adresinize gönderilen linke tıklayınız.</p>", "text/html; charset=utf-8");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View();
        }
    }
}
