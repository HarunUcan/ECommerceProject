using ECommerceProject.DtoLayer.Dtos.CartDtos;
using ECommerceProject.DtoLayer.Dtos.ProductDtos;
using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.PresentationLayer.ViewModels;

public class HomeViewModel
{
    public int? CurrentCategory { get; set; }
    public List<Product>? FeaturedProducts { get; set; }
    public List<Category>? Categories { get; set; }
    public List<Product>? FeaturedCategoryProducts { get; set; }
    public CartDetailDto? Cart { get; set; }
}
