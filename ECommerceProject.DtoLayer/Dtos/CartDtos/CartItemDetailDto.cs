using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.DtoLayer.Dtos.CartDtos;

public class CartItemDetailDto
{
    public int CartItemId { get; set; }
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountRate { get; set; }
    public decimal? DiscountAmount { get; set; }
    public int Quantity { get; set; }
    public ProductSize Size { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public IList<string> ImageUrls { get; set; } = new List<string>();
}
