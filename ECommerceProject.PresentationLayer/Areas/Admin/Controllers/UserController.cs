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

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userRoles = new List<(AppUser User, IList<string> Roles)>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add((user, roles));
            }

            var adminDtos = userRoles
                .Where(ur => ur.Roles.Contains("Admin"))
                .Select(ur => new AppUserDto
                {
                    Id = ur.User.Id,
                    Name = ur.User.Name,
                    Surname = ur.User.Surname,
                    Email = ur.User.Email,
                    PhoneNumber = ur.User.PhoneNumber,
                    IsActive = true,
                    Role = ur.Roles.FirstOrDefault()
                });

            var memberDtos = userRoles
                .Where(ur => !ur.Roles.Contains("Admin"))
                .Select(ur => new AppUserDto
                {
                    Id = ur.User.Id,
                    Name = ur.User.Name,
                    Surname = ur.User.Surname,
                    Email = ur.User.Email,
                    PhoneNumber = ur.User.PhoneNumber,
                    IsActive = true,
                    Role = ur.Roles.FirstOrDefault()
                });

            var appUserDtos = adminDtos.Concat(memberDtos).ToList();
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
