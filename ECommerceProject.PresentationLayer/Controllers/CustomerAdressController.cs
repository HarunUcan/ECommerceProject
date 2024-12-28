using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.BusinessLayer.Concrete;
using ECommerceProject.DataAccessLayer.Concrete;
using ECommerceProject.DtoLayer.Dtos.AdressDtos;
using ECommerceProject.EntityLayer.Concrete;
using ECommerceProject.PresentationLayer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProject.PresentationLayer.Controllers
{
    [Authorize]
    public class CustomerAdressController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAdressService _adressService;

        public CustomerAdressController(UserManager<AppUser> userManager, IAdressService adressService)
        {
            _userManager = userManager;
            _adressService = adressService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }
            List<Adress> adresses = _adressService.TGetAdressesByUserId(user.Id);
            UserAdressViewModel userAddressViewModel = new UserAdressViewModel
            {
                AdressList = adresses.Select(x => new AdressDto
                {
                    AdressId = x.AdressId,
                    Title = x.Title,
                    City = x.City,
                    District = x.District,
                    AdressLine = x.AdressLine
                }).ToList(),
                NewAdress = new AdressDto()
            };
            return View(userAddressViewModel);
        }

        [HttpPost]
        public IActionResult Index(UserAdressViewModel userAdressViewModel)
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid) {
                
                Adress adress = new Adress
                {
                    Title = userAdressViewModel.NewAdress.Title,
                    City = userAdressViewModel.NewAdress.City,
                    District = userAdressViewModel.NewAdress.District,
                    AdressLine = userAdressViewModel.NewAdress.AdressLine,
                    AppUserId = user.Id
                };
                _adressService.TInsert(adress);
                return RedirectToAction("Index", "CustomerAdress");
            }
            userAdressViewModel.AdressList = _adressService.TGetAdressesByUserId(user.Id)
                                          .Select(a => new AdressDto
                                          {
                                              Title = a.Title,
                                              City = a.City,
                                              District = a.District,
                                              AdressLine = a.AdressLine
                                          }).ToList();
            return View("Index", userAdressViewModel);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var userAdressList = _adressService.TGetAdressesByUserId(user.Id);
            if (userAdressList.All(x => x.AdressId != id))
            {
                return RedirectToAction("Index", "CustomerAdress");
            }
            _adressService.TDelete(new Adress { AdressId = id });
            return RedirectToAction("Index", "CustomerAdress");
        }
    }
}
