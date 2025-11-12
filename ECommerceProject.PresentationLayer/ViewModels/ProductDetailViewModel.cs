using ECommerceProject.DtoLayer.Dtos.ProductDtos;
using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.PresentationLayer.ViewModels
{
    public class ProductDetailViewModel
    {
        public ProductDto? ProductDto { get; set; }
        public List<Category>? Categories { get; set; }
        public Basket? Basket { get; set; }
    }
}
