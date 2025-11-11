namespace ECommerceProject.DtoLayer.Dtos.CartDtos;

public class CartDetailDto
{
    public int CartId { get; set; }
    public DateTime UpdatedDate { get; set; }
    public int? AppUserId { get; set; }
    public string? TempUserId { get; set; }
    public IList<CartItemDetailDto> Items { get; set; } = new List<CartItemDetailDto>();
    public IList<CartCouponDetailDto> Coupons { get; set; } = new List<CartCouponDetailDto>();
}
