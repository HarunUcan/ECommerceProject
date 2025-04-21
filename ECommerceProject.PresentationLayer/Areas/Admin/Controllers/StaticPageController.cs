using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.DtoLayer.Dtos.ProductDtos;
using ECommerceProject.DtoLayer.Dtos.StaticPageDtos;
using ECommerceProject.EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProject.PresentationLayer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class StaticPageController : Controller
    {
        private readonly IStaticPageService _staticPageService;

        public StaticPageController(IStaticPageService staticPageService)
        {
            _staticPageService = staticPageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PrivacyPolicy()
        {
            var staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.PrivacyPolicy);
            StaticPageDto staticPageDto;

            if (staticPage == null)
            {
                staticPageDto = new StaticPageDto
                {
                    Title = "",
                    Content = "",
                    UpdatedDate = DateTime.Now
                };
            }
            else
            {
                staticPageDto = new StaticPageDto
                {
                    Id = staticPage.Id,
                    Title = staticPage.Title,
                    Content = staticPage.Content,
                    UpdatedDate = staticPage.UpdatedDate
                };
            }
            return View(staticPageDto);
        }

        [HttpPost]
        public async Task<IActionResult> PrivacyPolicy(StaticPageDto staticPageDto)
        {
            if (ModelState.IsValid)
            {
                StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.PrivacyPolicy);
                if (staticPage == null)
                {
                    StaticPage newStaticPage = new StaticPage
                    {
                        Title = staticPageDto.Title,
                        Content = staticPageDto.Content,
                        StaticPageType = StaticPageType.PrivacyPolicy,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now
                    };
                    _staticPageService.TInsert(newStaticPage);
                }
                else
                {
                    staticPage.Title = staticPageDto.Title;
                    staticPage.Content = staticPageDto.Content;
                    staticPage.UpdatedDate = DateTime.Now;
                    _staticPageService.TUpdate(staticPage);
                }

                return View(staticPageDto);
            }
            return View(staticPageDto);
        }
    }
}
