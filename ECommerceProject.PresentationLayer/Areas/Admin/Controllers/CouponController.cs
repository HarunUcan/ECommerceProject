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
            var coupons = _couponService.TGetList();
            List<CouponDto> couponDtos = new List<CouponDto>();
            foreach (var coupon in coupons)
            {
                CouponDto couponDto = new CouponDto
                {
                    Id = coupon.CouponId,
                    Code = coupon.Code,
                    Discount = coupon.DiscountAmount ?? coupon.DiscountPercentage,
                    MinOrderAmount = coupon.MinOrderAmount,
                    ExpirationDate = coupon.ExpirationDate,
                    MaxUsageCount = coupon.MaxUsageCount,
                    CouponType = coupon.DiscountAmount != null ? "amount" : "percentage",
                    RemainingUsageCount = coupon.MaxUsageCount - coupon.CurrentUsageCount,
                    IsActive = coupon.IsActive
                };
                couponDtos.Add(couponDto);
            }
            return View(couponDtos);
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

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var coupon = _couponService.TGetById(id);
            _couponService.TDelete(coupon);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ChangeStatus(int id)
        {
            var coupon = _couponService.TGetById(id);
            coupon.IsActive = !coupon.IsActive;
            _couponService.TUpdate(coupon);
            return RedirectToAction("Index");
        }
    }
}
