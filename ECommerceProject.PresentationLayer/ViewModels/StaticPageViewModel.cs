using ECommerceProject.DtoLayer.Dtos.StaticPageDtos;
using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.PresentationLayer.ViewModels
{
    public class StaticPageViewModel
    {
        public List<Category>? Categories { get; set; }
        public Basket? Basket { get; set; }
        public StaticPageDto? StaticPageDto { get; set; }
    }
}
