namespace ECommerceProject.DtoLayer.Dtos.CartDtos;

public class CartCouponDetailDto
{
    public int CouponId { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal? DiscountAmount { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsActive { get; set; }
}
