using ECommerceProject.DtoLayer.Dtos.StaticPageDtos;
using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.PresentationLayer.ViewModels
{
    public class StaticPageViewModel
    {
        public List<Category>? Categories { get; set; }
        public Cart? Cart { get; set; }
        public StaticPageDto? StaticPageDto { get; set; }
    }
}
