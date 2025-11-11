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



        [HttpGet]
        public async Task<IActionResult> CookiePolicy()
        {
            var staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.CookiePolicy);
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
        public async Task<IActionResult> CookiePolicy(StaticPageDto staticPageDto)
        {
            if (ModelState.IsValid)
            {
                StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.CookiePolicy);
                if (staticPage == null)
                {
                    StaticPage newStaticPage = new StaticPage
                    {
                        Title = staticPageDto.Title,
                        Content = staticPageDto.Content,
                        StaticPageType = StaticPageType.CookiePolicy,
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



        [HttpGet]
        public async Task<IActionResult> DistanceSalesAgreement()
        {
            var staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.DistanceSalesAgreement);
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
        public async Task<IActionResult> DistanceSalesAgreement(StaticPageDto staticPageDto)
        {
            if (ModelState.IsValid)
            {
                StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.DistanceSalesAgreement);
                if (staticPage == null)
                {
                    StaticPage newStaticPage = new StaticPage
                    {
                        Title = staticPageDto.Title,
                        Content = staticPageDto.Content,
                        StaticPageType = StaticPageType.DistanceSalesAgreement,
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



        [HttpGet]
        public async Task<IActionResult> ReturnAndRefundPolicy()
        {
            var staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.ReturnAndRefundPolicy);
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
        public async Task<IActionResult> ReturnAndRefundPolicy(StaticPageDto staticPageDto)
        {
            if (ModelState.IsValid)
            {
                StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.ReturnAndRefundPolicy);
                if (staticPage == null)
                {
                    StaticPage newStaticPage = new StaticPage
                    {
                        Title = staticPageDto.Title,
                        Content = staticPageDto.Content,
                        StaticPageType = StaticPageType.ReturnAndRefundPolicy,
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



        [HttpGet]
        public async Task<IActionResult> MembershipAgreement()
        {
            var staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.MembershipAgreement);
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
        public async Task<IActionResult> MembershipAgreement(StaticPageDto staticPageDto)
        {
            if (ModelState.IsValid)
            {
                StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.MembershipAgreement);
                if (staticPage == null)
                {
                    StaticPage newStaticPage = new StaticPage
                    {
                        Title = staticPageDto.Title,
                        Content = staticPageDto.Content,
                        StaticPageType = StaticPageType.MembershipAgreement,
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



        [HttpGet]
        public async Task<IActionResult> KVKK()
        {
            var staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.KVKK);
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
        public async Task<IActionResult> KVKK(StaticPageDto staticPageDto)
        {
            if (ModelState.IsValid)
            {
                StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.KVKK);
                if (staticPage == null)
                {
                    StaticPage newStaticPage = new StaticPage
                    {
                        Title = staticPageDto.Title,
                        Content = staticPageDto.Content,
                        StaticPageType = StaticPageType.KVKK,
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



        [HttpGet]
        public async Task<IActionResult> AboutUs()
        {
            var staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.AboutUs);
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
        public async Task<IActionResult> AboutUs(StaticPageDto staticPageDto)
        {
            if (ModelState.IsValid)
            {
                StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.AboutUs);
                if (staticPage == null)
                {
                    StaticPage newStaticPage = new StaticPage
                    {
                        Title = staticPageDto.Title,
                        Content = staticPageDto.Content,
                        StaticPageType = StaticPageType.AboutUs,
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



        [HttpGet]
        public async Task<IActionResult> Store()
        {
            var staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.Store);
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
        public async Task<IActionResult> Store(StaticPageDto staticPageDto)
        {
            if (ModelState.IsValid)
            {
                StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.Store);
                if (staticPage == null)
                {
                    StaticPage newStaticPage = new StaticPage
                    {
                        Title = staticPageDto.Title,
                        Content = staticPageDto.Content,
                        StaticPageType = StaticPageType.Store,
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



        [HttpGet]
        public async Task<IActionResult> FAQ()
        {
            var staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.FAQ);
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
        public async Task<IActionResult> FAQ(StaticPageDto staticPageDto)
        {
            if (ModelState.IsValid)
            {
                StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.FAQ);
                if (staticPage == null)
                {
                    StaticPage newStaticPage = new StaticPage
                    {
                        Title = staticPageDto.Title,
                        Content = staticPageDto.Content,
                        StaticPageType = StaticPageType.FAQ,
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



        [HttpGet]
        public async Task<IActionResult> PaymentOptions()
        {
            var staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.PaymentOptions);
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
        public async Task<IActionResult> PaymentOptions(StaticPageDto staticPageDto)
        {
            if (ModelState.IsValid)
            {
                StaticPage staticPage = await _staticPageService.TGetByEnumTypeAsync(StaticPageType.PaymentOptions);
                if (staticPage == null)
                {
                    StaticPage newStaticPage = new StaticPage
                    {
                        Title = staticPageDto.Title,
                        Content = staticPageDto.Content,
                        StaticPageType = StaticPageType.PaymentOptions,
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
