using ECommerceProject.EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProject.PresentationLayer.Controllers
{
    public class ConfirmMailController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public ConfirmMailController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int userId, string token)
        {
            var appUser = await _userManager.FindByIdAsync(userId.ToString());
            if (appUser != null)
            {
                var confirmationResult = await _userManager.ConfirmEmailAsync(appUser, token);
                if (confirmationResult.Succeeded)
                {
                    return View();
                }
            }
            return Content("Doğrulama başarısız.");
        }
    }
}
