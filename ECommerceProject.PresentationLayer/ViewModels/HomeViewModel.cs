using ECommerceProject.DtoLayer.Dtos.ProductDtos;
using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.PresentationLayer.ViewModels
{
    public class HomeViewModel
    {
        public int? CurrentCategory { get; set; } // Categorye bağlı ürünleri Listelemek için yazıldı muhtemelen Silinecek
        public List<Product>? FeaturedProducts { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Product>? FeaturedCategoryProducts { get; set; }
        public Basket? Basket { get; set; }
    }
}
