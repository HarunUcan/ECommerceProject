using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.DtoLayer.Dtos.CouponDtos;
using ECommerceProject.EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProject.PresentationLayer.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CouponDto couponDto)
        {
            if (ModelState.IsValid)
            {
                if (couponDto.CouponType == "percentage") // %
                {
                    _couponService.TInsert(new Coupon
                    {
                        Code = couponDto.Code,
                        DiscountPercentage = couponDto.Discount,
                        MinOrderAmount = couponDto.MinOrderAmount,
                        ExpirationDate = couponDto.ExpirationDate,
                        MaxUsageCount = couponDto.MaxUsageCount,
                        CurrentUsageCount = 0,
                        IsActive = true
                    });
                }
                else if(couponDto.CouponType == "amount") // TL
                {
                    _couponService.TInsert(new Coupon
                    {
                        Code = couponDto.Code,
                        DiscountAmount = couponDto.Discount,
                        MinOrderAmount = couponDto.MinOrderAmount,
                        ExpirationDate = couponDto.ExpirationDate,
                        MaxUsageCount = couponDto.MaxUsageCount,
                        CurrentUsageCount = 0,
                        IsActive = true
                    });
                }
            }
            return View(couponDto);
        }
    }
}
