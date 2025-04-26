using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.DtoLayer.Dtos.AppUserDtos;
using ECommerceProject.EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProject.PresentationLayer.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ICartService _cartService;
        private readonly IAppUserService _userService;

        public UserController(UserManager<AppUser> userManager, ICartService cartService, IAppUserService userService)
        {
            _userManager = userManager;
            _cartService = cartService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            List<AppUserDto> appUserDtos = new List<AppUserDto>();

            // önce adminleri listeye ekliyoruz
            foreach (var user in users)
            {
                var roles = _userManager.GetRolesAsync(user).Result;
                if (roles.Contains("Admin"))
                {
                    AppUserDto appUserDto = new AppUserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Surname = user.Surname,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        IsActive = true,
                        //CreatedDate = user.CreatedDate,
                        Role = roles.FirstOrDefault()
                    };
                    appUserDtos.Add(appUserDto);
                }
            }

            // sonra admin olmayanları listeye ekliyoruz
            foreach (var user in users)
            {
                var roles = _userManager.GetRolesAsync(user).Result;
                if (!roles.Contains("Admin"))
                {
                    AppUserDto appUserDto = new AppUserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Surname = user.Surname,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        IsActive = true,
                        //CreatedDate = user.CreatedDate,
                        Role = roles.FirstOrDefault()
                    };
                    appUserDtos.Add(appUserDto);
                }
            }

            return View(appUserDtos);
        }

        [HttpGet]
        public async Task<IActionResult> SearchUsers(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<object>());

            var users = await _userService.TGetUsersBySearchAsync(term); // Serviste arama metodu
            var result = users.Select(u => new
            {
                id = u.Id,
                name = u.Name,
                surname = u.Surname,
                email = u.Email
            });

            return Json(result);
        }
    }

    

}
