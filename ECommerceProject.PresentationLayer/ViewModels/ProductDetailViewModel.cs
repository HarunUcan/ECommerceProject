using ECommerceProject.DtoLayer.Dtos.ProductDtos;
using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.PresentationLayer.ViewModels
{
    public class ProductDetailViewModel
    {
        public List<Category>? Categories { get; set; }
        public ProductDto? ProductDto { get; set; }
    }
}
