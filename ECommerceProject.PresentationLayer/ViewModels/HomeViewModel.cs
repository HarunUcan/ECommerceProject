using ECommerceProject.EntityLayer.Concrete;

namespace ECommerceProject.PresentationLayer.ViewModels
{
    public class HomeViewModel
    {
        public List<Product>? Products { get; set; } // Categorye bağlı ürünleri Listelemek için yazıldı muhtemelen Silinecek
        public int? CurrentCategory { get; set; } // Categorye bağlı ürünleri Listelemek için yazıldı muhtemelen Silinecek
        public List<Product>? FeaturedProducts { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Product>? FeaturedCategoryProducts { get; set; }
        public Cart? Cart { get; set; }
    }
}
