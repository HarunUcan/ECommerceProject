using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceProject.DtoLayer.Dtos.CouponDtos
{
    public class CouponDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal? Discount { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int MaxUsageCount { get; set; }
        public string CouponType { get; set; }
        public int RemainingUsageCount { get; set; }
        public bool IsActive { get; set; }
    }
}
