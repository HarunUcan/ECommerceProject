using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.DtoLayer.Dtos.AppUserDtos;
using ECommerceProject.EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProject.PresentationLayer.Areas.User.Controllers
{
    [Area("User")]
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMailSenderService _confirmationMailSenderService;

        public RegisterController(UserManager<AppUser> userManager, IMailSenderService confirmationMailSenderService)
        {
            _userManager = userManager;
            _confirmationMailSenderService = confirmationMailSenderService;
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
                if (appUserRegisterDto.Password != appUserRegisterDto.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Şifreler uyuşmuyor.");
                    return View(appUserRegisterDto);
                }
                if (!appUserRegisterDto.IsAgreeToUserAgreement)
                {
                    ModelState.AddModelError("", "Kullanıcı sözleşmesini kabul etmelisiniz.");
                    return View(appUserRegisterDto);
                }
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
                    var mailConfToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);

                    var confirmationLink = Url.Action("Index", "ConfirmMail", new
                    {
                        userId = appUser.Id,
                        token = mailConfToken
                    }, Request.Scheme);

                    var mailBody = PopulateEmailBody(appUser.UserName, confirmationLink);
                    await _confirmationMailSenderService.SendConfirmationMailAsync(appUser.Email, "Hesabınızı Aktifleştirin", mailBody);

                    return Content($"<h1>Son Bir Adım Kaldı!</h1> <p>Kaydınız başarılı bir şekilde gerçekleşti. Hesabınızı aktifleştirmek için mail adresinize gönderilen linke tıklayınız.</p>", "text/html; charset=utf-8");
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

        private string PopulateEmailBody(string userName, string link)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader("wwwroot/EmailTemplates/ConfirmationMail.html"))
            {
                body = reader.ReadToEnd();
            }
            //body = body.Replace("{UserName}", userName);
            body = body.Replace("{ConfirmationLink}", link);
            return body;
        }
    }
}
