using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.DtoLayer.Dtos.AppUserDtos;
using ECommerceProject.EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProject.PresentationLayer.Areas.User.Controllers
{
    [Area("User")]
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        private readonly ICartService _cartService;

        public LoginController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, ICartService cartService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _cartService = cartService;
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
                        if(Request.Cookies["TempUserId"] != null)
                        {
                            // TODO: Geçici Sepet içeriğini kullanıcının sepetine aktar
                            string tempUserId = Request.Cookies["TempUserId"];
                            bool isTransferSuccess = _cartService.TTransferCart(tempUserId, user.Id);

                            // Geçici Sepeti sil
                            if (isTransferSuccess)
                                _cartService.TDeleteByTempUserId(tempUserId);

                            // Cookie yi sil
                            if (isTransferSuccess)
                                Response.Cookies.Delete("TempUserId");
                        }
                        return RedirectToAction("Index", "CustomerProfile");
                        //return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View(appUserLoginDto);
        }
    }
}
