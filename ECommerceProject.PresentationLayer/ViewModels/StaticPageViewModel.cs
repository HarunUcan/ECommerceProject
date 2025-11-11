using ECommerceProject.DtoLayer.Dtos.CartDtos;
using ECommerceProject.DtoLayer.Dtos.StaticPageDtos;
using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.PresentationLayer.ViewModels;

public class StaticPageViewModel
{
    public List<Category>? Categories { get; set; }
    public CartDetailDto? Cart { get; set; }
    public StaticPageDto? StaticPageDto { get; set; }
}
